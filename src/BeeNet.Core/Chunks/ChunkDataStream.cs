// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Chunks
{
    public sealed class ChunkDataStream : Stream
    {
        // Fields.
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly uint dataSegmentSize;
        private readonly Hasher hasher = new();
        private List<SwarmCac> levelsCache = [];
        private readonly uint maxDataSegmentsInChunk;
        private readonly RedundancyStrategy redundancyStrategy;
        private readonly bool redundancyStrategyFallback;
        private readonly SwarmCac rootChunk;
        private readonly int rootParities;
        private readonly SwarmReference rootReference;

        // Constructor.
        private ChunkDataStream(
            SwarmCac rootChunk,
            SwarmReference rootReference,
            IReadOnlyChunkStore chunkStore,
            long length,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback)
        {
            if (rootChunk.Hash != rootReference.Hash)
                throw new ArgumentException("root chunk hash does not match root reference hash");

            this.chunkStore = chunkStore;
            this.rootChunk = rootChunk;
            this.rootReference = rootReference;
            Length = length;
            Position = 0;

            // Define segments info.
            dataSegmentSize = (uint)rootReference.Size;

            if (rootChunk.RedundancyLevel == RedundancyLevel.None)
            {
                rootParities = 0;
                maxDataSegmentsInChunk = SwarmCac.DataSize / dataSegmentSize;

                // If root chunk has no redundancy, strategy is ignored and set to DATA without fallback.
                this.redundancyStrategy = RedundancyStrategy.Data;
                this.redundancyStrategyFallback = false;
            }
            else
            {
                var (_, parities) = rootChunk.CountIntermediateReferences(rootReference.IsEncrypted);
                rootParities = parities;
                maxDataSegmentsInChunk = (uint)rootChunk.RedundancyLevel.GetMaxDataShards(rootReference.IsEncrypted);
                
                this.redundancyStrategy = redundancyStrategy;
                this.redundancyStrategyFallback = redundancyStrategyFallback;
            }
        }

        // Static builder.
        public static async Task<Stream> BuildNewAsync(
            SwarmReference reference,
            IReadOnlyChunkStore chunkStore,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback)
        {
            ArgumentNullException.ThrowIfNull(chunkStore, nameof(chunkStore));
            
            // Use chunk redundancy resolver if required.
            var rootChunkStore = redundancyLevel == RedundancyLevel.None ?
                chunkStore :
                new ReplicaResolverChunkStore(chunkStore, redundancyLevel, new Hasher());
            
            // Resolve root chunk.
            var rootChunk = await rootChunkStore.GetAsync(reference.Hash).ConfigureAwait(false);
            if (rootChunk is not SwarmCac rootCac) //soc are not supported
                throw new InvalidOperationException($"Chunk {reference} is not a Content Addressed Chunk.");
            
            return BuildNew(
                rootCac,
                reference,
                chunkStore,
                redundancyStrategy,
                redundancyStrategyFallback);
        }

        public static Stream BuildNew(
            SwarmCac rootChunk,
            IReadOnlyChunkStore chunkStore,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback,
            EncryptionKey256? encryptionKey = null)
        {
            ArgumentNullException.ThrowIfNull(rootChunk, nameof(rootChunk));
            return BuildNew(
                rootChunk,
                new SwarmReference(rootChunk.Hash, encryptionKey),
                chunkStore,
                redundancyStrategy,
                redundancyStrategyFallback);
        }
        
        public static Stream BuildNew(
            SwarmCac rootChunk,
            SwarmReference reference,
            IReadOnlyChunkStore chunkStore,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback)
        {
            ArgumentNullException.ThrowIfNull(rootChunk, nameof(rootChunk));

            ulong length;
            if (reference.IsEncrypted)
            {
                var spanBuffer = new byte[SwarmCac.SpanSize];
                var dataBuffer = new byte[SwarmCac.DataSize];
                ChunkEncrypter.DecryptChunk(
                    rootChunk,
                    reference.EncryptionKey!.Value,
                    spanBuffer,
                    dataBuffer,
                    new Hasher());
                length =  SwarmCac.SpanToLength(spanBuffer);
            }
            else length =  SwarmCac.SpanToLength(rootChunk.Span.Span);

            return new ChunkDataStream(
                rootChunk,
                reference,
                chunkStore,
                (long)length,
                redundancyStrategy,
                redundancyStrategyFallback);
        }
        
        // Properties.
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length { get; }
        public override long Position { get; set; }

        // Methods.
        public override void Flush() { }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            var task = ReadAsync(buffer, offset, count);
            task.Wait();
            return task.Result;
        }
        
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            await ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);
        
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            // Return 0 when the stream is at the end.
            var dataToRead = (int)Math.Min(buffer.Length, Length - Position);
            if (dataToRead == 0)
                return 0;
            
            // Read data from chunks.
            await CopyDataToBufferAsync(buffer[..dataToRead], cancellationToken).ConfigureAwait(false);
            
            Position += dataToRead;
            return dataToRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var newPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentException("Invalid seek origin", nameof(origin))
            };

            if (newPosition < 0)
                throw new IOException("Cannot seek before the beginning of the stream");
            if (newPosition > Length)
                throw new IOException("Cannot seek past the end of the stream");

            Position = newPosition;
            return Position;
        }

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        
        // Private helpers.
        private async Task CopyDataToBufferAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            List<SwarmCac> newLevelsCache = [];
            
            // Init with root level info.
            var levelStartDataOffset = (ulong)Position;
            var levelEndDataOffset = (ulong)(Length - (Position + buffer.Length));
            (SwarmCac Chunk, SwarmReference Reference)[] levelChunkReferencePairs = [new(rootChunk, rootReference)];
            
            // Reuse these for memory optimization.
            var chunkSpanBuffer = new byte[SwarmCac.SpanSize];
            var chunkDataBuffer = new byte[SwarmCac.DataSize];
            var levelChildReferences = new List<SwarmReference>();

            // Iterate on all levels from root to data chunks. Terminate when no chunks remain.
            for (var levelIndex = 0;; levelIndex++)
            {
                // Init.
                levelChildReferences.Clear();
                //optimize value search for chunks in level. Analyzing from right, it can only be monotonically non-decreasing
                ulong dataSizeBySegment = SwarmCac.DataSize;

                // Parse all chunks on current level. Start from end to copy data first, if present in level.
                for (int chunkIndex = levelChunkReferencePairs.Length - 1; chunkIndex >= 0; chunkIndex--)
                {
                    var (chunk, reference) = levelChunkReferencePairs[chunkIndex];
                    var isFirstChunkInLevel = chunkIndex == 0;
                    var isLastChunkInLevel = chunkIndex == levelChunkReferencePairs.Length - 1;

                    // If it's the last chunk on the level, and if some data remains to read in level, cache the chunk.
                    if (isLastChunkInLevel && levelEndDataOffset > 0)
                        newLevelsCache.Add(chunk);
                    
                    // Decrypt chunk's data.
                    int dataLength;
                    if (reference.IsEncrypted)
                    {
                        dataLength = ChunkEncrypter.DecryptChunk(
                            chunk,
                            reference.EncryptionKey!.Value,
                            chunkSpanBuffer,
                            chunkDataBuffer,
                            hasher);
                    }
                    else
                    {
                        chunk.Span.CopyTo(chunkSpanBuffer);
                        chunk.Data.CopyTo(chunkDataBuffer);
                        dataLength = chunk.Data.Length;
                    }
                    
                    // If is a data chunk, report data on buffer and update bounds. Then continue.
                    var referredDataSize = SwarmCac.SpanToLength(chunkSpanBuffer);
                    if (referredDataSize <= SwarmCac.DataSize)
                    {
                        //check end offset consistency
                        var dataToCopyStart = isFirstChunkInLevel ? (int)levelStartDataOffset : 0;
                        var dataToCopySize = dataLength - dataToCopyStart - (int)levelEndDataOffset;
                        if (dataToCopySize <= 0)
                            throw new InvalidOperationException("Invalid data to copy size");
                        
                        //copy data to end of buffer, and shrink buffer
                        chunkDataBuffer[dataToCopyStart..(dataToCopyStart + dataToCopySize)].CopyTo(buffer[^dataToCopySize..]);
                        buffer = buffer[..^dataToCopySize];
                        
                        //update level bounds
                        levelEndDataOffset = 0;

                        continue;
                    }

                    // Else if it's an intermediate chunk
                    if (dataLength % dataSegmentSize != 0)
                        throw new InvalidOperationException("Intermediate chunk's data length is not multiple of segment size.");
            
                    // Find referred data size by segment.
                    var segmentsAmount = (uint)(dataLength / dataSegmentSize);
                    while (dataSizeBySegment * segmentsAmount < referredDataSize)
                        dataSizeBySegment *= maxDataSegmentsInChunk;
                    
                    // Define chunk's segments to read and set bounds for the next level.
                    var chunkStartPosition = 0;
                    if (isFirstChunkInLevel)
                    {
                        var startSegmentsToSkip = levelStartDataOffset / dataSizeBySegment;
                        chunkStartPosition = (int)(startSegmentsToSkip * dataSegmentSize);
                        levelStartDataOffset -= startSegmentsToSkip * dataSizeBySegment;
                    }
                    var chunkEndPosition = dataLength;
                    if (isLastChunkInLevel)
                    {
                        var lastPartialSegmentDataSize = referredDataSize % dataSizeBySegment;
                        ulong endSegmentsToSkip = 0;
                        if (levelEndDataOffset >= lastPartialSegmentDataSize)
                        {
                            endSegmentsToSkip = (levelEndDataOffset - lastPartialSegmentDataSize) / dataSizeBySegment;
                            if (lastPartialSegmentDataSize > 0)
                                endSegmentsToSkip++;
                        }
                        chunkEndPosition = dataLength - (int)(endSegmentsToSkip * dataSegmentSize);
                        if (endSegmentsToSkip > 0)
                            levelEndDataOffset -= referredDataSize % dataSizeBySegment == 0
                                ? endSegmentsToSkip * dataSizeBySegment
                                : (endSegmentsToSkip - 1) * dataSizeBySegment + referredDataSize % dataSizeBySegment;
                    }

                    // Reverse read child chunks references and prepend them on reference list.
                    for (var cursor = chunkEndPosition; cursor > chunkStartPosition;)
                    {
                        cursor -= (int)dataSegmentSize;
                        levelChildReferences.Insert(0, 
                            new SwarmReference(chunkDataBuffer.Slice(cursor, cursor + (int)dataSegmentSize)));
                    }
                }
                
                // If next level is empty, set cache and return.
                if (levelChildReferences.Count == 0)
                {
                    // Verify the full buffer has been written.
                    if (buffer.Length != 0)
                        throw new InvalidOperationException("Not full buffer has been written");

                    // Replace cache.
                    levelsCache = newLevelsCache;

                    return;
                }

                // Search chunks for the next level.
                IReadOnlyDictionary<SwarmHash, SwarmChunk> childChunksPool;
                
                //with cache
                if (levelsCache.Count > levelIndex + 1 &&
                    levelChildReferences.Any(r => r.Hash == levelsCache[levelIndex + 1].Hash))
                {
                    var hashesToGetFromStore = levelChildReferences
                        .Select(p => p.Hash)
                        .Where(h => h != levelsCache[levelIndex + 1].Hash).ToArray();
                    IEnumerable<KeyValuePair<SwarmHash, SwarmChunk>> chunksFromStore =
                        hashesToGetFromStore.Length != 0
                            ? await chunkStore.GetAsync(hashesToGetFromStore, cancellationToken: cancellationToken).ConfigureAwait(false)
                            : Array.Empty<KeyValuePair<SwarmHash, SwarmChunk>>();
                    
                    childChunksPool = new Dictionary<SwarmHash, SwarmChunk>(chunksFromStore)
                    {
                        [levelsCache[levelIndex + 1].Hash] = levelsCache[levelIndex + 1]
                    };
                }
                else //or without cache
                {
                    childChunksPool = await chunkStore.GetAsync(
                        levelChildReferences.Select(p => p.Hash),
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                
                // Resolve child chunks from the reference list.
                levelChunkReferencePairs = levelChildReferences.Select(reference =>
                    ((SwarmCac)childChunksPool[reference.Hash], reference)).ToArray();
            }
        }
    }
}