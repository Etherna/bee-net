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
        // Internal classes.
        private record ChunkInfo(
            SwarmReference Reference,
            SwarmCac Chunk,
            RedundancyLevel RedundancyLevel);
        
        // Fields.
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly uint dataSegmentSize;
        private readonly Hasher hasher = new();
        private List<SwarmCac> levelsCache = [];
        private readonly uint maxDataSegmentsInChunk;
        private readonly RedundancyStrategy redundancyStrategy;
        private readonly bool redundancyStrategyFallback;
        private readonly ChunkInfo rootChunkInfo;

        // Constructor.
        private ChunkDataStream(
            ChunkInfo rootChunkInfo,
            IReadOnlyChunkStore chunkStore,
            long length,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback)
        {
            this.chunkStore = chunkStore;
            Length = length;
            Position = 0;
            this.rootChunkInfo = rootChunkInfo;

            // Define segments info.
            dataSegmentSize = (uint)rootChunkInfo.Reference.Size;
            if (rootChunkInfo.RedundancyLevel == RedundancyLevel.None)
            {
                maxDataSegmentsInChunk = SwarmCac.DataSize / dataSegmentSize;

                // If root chunk has no redundancy, strategy is ignored and set to DATA without fallback.
                this.redundancyStrategy = RedundancyStrategy.Data;
                this.redundancyStrategyFallback = false;
            }
            else
            {
                maxDataSegmentsInChunk = (uint)rootChunkInfo.RedundancyLevel.GetMaxDataShards(
                    rootChunkInfo.Reference.IsEncrypted);
                
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
            ArgumentNullException.ThrowIfNull(chunkStore);
            
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
            ArgumentNullException.ThrowIfNull(rootChunk);
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
            ArgumentNullException.ThrowIfNull(rootChunk);
            
            if (rootChunk.Hash != reference.Hash)
                throw new ArgumentException("root chunk hash does not match root reference hash");

            // Extract and decrypt span.
            byte[] spanBuffer;
            if (reference.IsEncrypted)
            {
                spanBuffer = new byte[SwarmCac.SpanSize];
                var dataBuffer = new byte[SwarmCac.DataSize];
                ChunkEncrypter.DecryptChunk(
                    rootChunk,
                    reference.EncryptionKey!.Value,
                    spanBuffer,
                    dataBuffer,
                    new Hasher());
            }
            else
            {
                spanBuffer = rootChunk.Span.ToArray();
            }
            
            // Extract redundancy level, decode span, then extract data length.
            var redundancyLevel = SwarmCac.GetSpanRedundancyLevel(spanBuffer);
            
            if (SwarmCac.IsSpanEncoded(spanBuffer))
                SwarmCac.DecodeSpan(spanBuffer);
                
            var length = SwarmCac.SpanToLength(spanBuffer);

            // Build stream.
            return new ChunkDataStream(
                new ChunkInfo(reference, rootChunk, redundancyLevel),
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
            ChunkInfo[] levelChunksInfo = [rootChunkInfo];
            
            // Reuse these for memory optimization.
            var chunkSpanBuffer = new byte[SwarmCac.SpanSize];
            var chunkDataBuffer = new byte[SwarmCac.DataSize];
            
            /*
             * In the specific case where chunks have parities, these needs to be resolved with ParityDecoderChunkStore.
             * The decoder implements IReadOnlyChunkStore, and will try to fetch and recover child chunks.
             * Each decoder is responsible for decoding a single chunk, so we need a list of decoders for each level.
             * Associated with each decoder there is also a list of references to explore, resolvable from that
             * specific decoder.
             * In case redundancy is not involved, the IReadOnlyChunkStore will simply be the source chunk store.
             */
            List<(IReadOnlyChunkStore Store, SwarmReference[] References)> chunkStoresWithReferences = [];
            List<Task> fetchAndRecoverTasks = [];

            // Iterate on all levels from root to data chunks. Terminate when no chunks remain.
            for (var levelIndex = 0;; levelIndex++)
            {
                // Init level.
                chunkStoresWithReferences.Clear();
                fetchAndRecoverTasks.Clear();
                
                // Optimize value search for chunks in level.
                // Analyzing from right, it can only be monotonically non-decreasing.
                ulong dataSizeBySegment = SwarmCac.DataSize;

                // Parse all chunks on current level. Start from end to copy data first, if present in level.
                for (int chunkIndex = levelChunksInfo.Length - 1; chunkIndex >= 0; chunkIndex--)
                {
                    var (reference, chunk, redundancyLevel) = levelChunksInfo[chunkIndex];
                    var isFirstChunkInLevel = chunkIndex == 0;
                    var isLastChunkInLevel = chunkIndex == levelChunksInfo.Length - 1;

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
                    var startSegmentsToSkip = 0;
                    var endSegmentsToSkip = 0;
                    if (isFirstChunkInLevel)
                    {
                        startSegmentsToSkip = (int)(levelStartDataOffset / dataSizeBySegment);
                        levelStartDataOffset -= (ulong)startSegmentsToSkip * dataSizeBySegment;
                    }
                    if (isLastChunkInLevel)
                    {
                        var lastPartialSegmentDataSize = referredDataSize % dataSizeBySegment;
                        if (levelEndDataOffset >= lastPartialSegmentDataSize)
                        {
                            endSegmentsToSkip = (int)((levelEndDataOffset - lastPartialSegmentDataSize) / dataSizeBySegment);
                            if (lastPartialSegmentDataSize > 0)
                                endSegmentsToSkip++;
                        }
                        if (endSegmentsToSkip > 0)
                            levelEndDataOffset -= referredDataSize % dataSizeBySegment == 0
                                ? (ulong)endSegmentsToSkip * dataSizeBySegment
                                : ((ulong)endSegmentsToSkip - 1) * dataSizeBySegment + referredDataSize % dataSizeBySegment;
                    }
                    
                    // Extract references, and if chunk has redundancy, fetch and resolve child chunks.
                    if (redundancyLevel == RedundancyLevel.None)
                    {
                        // Extract references.
                        var references = SwarmCac.GetIntermediateReferencesFromData(
                            chunkDataBuffer.AsSpan(0, dataLength), 0, reference.IsEncrypted);
                        
                        // Prepend references with chunk store on list.
                        chunkStoresWithReferences.Insert(0,
                            (chunkStore, references
                                .Select(r => r.Reference)
                                .Skip(startSegmentsToSkip)
                                .SkipLast(endSegmentsToSkip).ToArray()));
                    }
                    else
                    {
                        // Extract references.
                        var references = SwarmCac.GetIntermediateReferencesFromSpanData(
                            chunkSpanBuffer, chunkDataBuffer, reference.IsEncrypted);
                        
                        // Prepend references with decoder as chunk store on list.
                        var decoder = new ParityDecoderChunkStore(references, chunkStore);
                        chunkStoresWithReferences.Insert(0,
                            (decoder, references
                                .Select(r => r.Reference)
                                .Skip(startSegmentsToSkip)
                                .SkipLast(endSegmentsToSkip).ToArray()));
                        
                        // Run fetch and recover asynchronously.
                        fetchAndRecoverTasks.Add(decoder.FetchAndRecoverAsync(
                            redundancyStrategy,
                            redundancyStrategyFallback,
                            cancellationToken: cancellationToken));
                    }
                }
                
                // Wait all fetch and recover tasks for this level.
                await Task.WhenAll(fetchAndRecoverTasks).ConfigureAwait(false);
                
                // If next level is empty, set cache and return.
                if (chunkStoresWithReferences.Count == 0)
                {
                    // Verify the full buffer has been written.
                    if (buffer.Length != 0)
                        throw new InvalidOperationException("Not full buffer has been written");

                    // Replace cache.
                    levelsCache = newLevelsCache;

                    return;
                }

                // Search chunks for the next level.
                Dictionary<SwarmHash, SwarmChunk> childChunksPool;
                
                //with cache
                if (levelsCache.Count > levelIndex + 1 &&
                    levelChildReferences.Any(r => r.Hash == levelsCache[levelIndex + 1].Hash))
                {
                    var hashesToGetFromStore = levelChildReferences
                        .Select(p => p.Hash)
                        .Where(h => h != levelsCache[levelIndex + 1].Hash).ToArray();
                    var chunksFromStore = hashesToGetFromStore.Length == 0 ? [] :
                        (await chunkStore.GetAsync(hashesToGetFromStore, cancellationToken: cancellationToken).ConfigureAwait(false))
                            .Where(p => p.Value != null)
                            .Select(p => new KeyValuePair<SwarmHash, SwarmChunk>(p.Key, p.Value!));
                    
                    childChunksPool = new Dictionary<SwarmHash, SwarmChunk>(chunksFromStore)
                    {
                        [levelsCache[levelIndex + 1].Hash] = levelsCache[levelIndex + 1]
                    };
                }
                else //or without cache
                {
                    childChunksPool = (await chunkStore.GetAsync(
                            levelChildReferences.Select(p => p.Hash),
                            cancellationToken: cancellationToken).ConfigureAwait(false))
                        .Where(p => p.Value != null)
                        .ToDictionary(p => p.Key, p => p.Value!);
                }
                
                // Resolve child chunks from the reference list.
                levelChunksInfo = levelChildReferences.Select(reference =>
                    new ChunkInfo(reference, (SwarmCac)childChunksPool[reference.Hash])).ToArray();
            }
        }
    }
}