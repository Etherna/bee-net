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
        // Fields.
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly SwarmDecodedCac decodedRoot;
        private readonly Hasher hasher = new();
        private List<SwarmDecodedCac> levelsCache = [];
        private readonly uint maxDataSegmentsInChunk;
        private readonly RedundancyStrategy redundancyStrategy;
        private readonly bool redundancyStrategyFallback;

        // Constructor.
        private ChunkDataStream(
            SwarmDecodedCac decodedRoot,
            IReadOnlyChunkStore chunkStore,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback)
        {
            this.chunkStore = chunkStore;
            this.decodedRoot = decodedRoot;
            Position = 0;

            // Define segments info.
            if (decodedRoot.RedundancyLevel == RedundancyLevel.None)
            {
                maxDataSegmentsInChunk = SwarmCac.DataSize / (uint)decodedRoot.Reference.Size;

                // If root chunk has no redundancy, strategy is ignored and set to DATA without fallback.
                this.redundancyStrategy = RedundancyStrategy.Data;
                this.redundancyStrategyFallback = false;
            }
            else
            {
                maxDataSegmentsInChunk = (uint)decodedRoot.RedundancyLevel.GetMaxDataShards(
                    decodedRoot.Reference.IsEncrypted);
                
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
            return new ChunkDataStream(
                rootChunk.Decode(reference, new Hasher()),
                chunkStore,
                redundancyStrategy,
                redundancyStrategyFallback);
        }

        // Properties.
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => (long)decodedRoot.SpanLength;
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
            List<SwarmDecodedCac> newLevelsCache = [];
            
            // Init with root level info.
            var levelStartDataOffset = Position;
            var levelEndDataOffset = Length - (Position + buffer.Length);
            SwarmDecodedCac[] levelDecodedChunks = [decodedRoot];
            
            // Reuse these for memory optimization.
            /*
             * When chunks have parities, these needs to be resolved with a ChunkParityDecoder, that will try
             * to fetch and recover child chunks.
             * Each decoder is responsible for decoding a single chunk, so we need a list of decoders for each level.
             * Associated with each decoder there is also a list of references to explore, resolvable from that
             * specific decoder.
             */
            List<(ChunkParityDecoder Decoder, Dictionary<SwarmHash, SwarmReference> HashReferenceMap)> chunkStoresWithReferences = [];
            List<Task> fetchAndRecoverTasks = [];
            Dictionary<SwarmReference, SwarmDecodedCac> referenceDecodedChunkMap = [];

            // Iterate on all levels from root to data chunks. Terminate when no chunks remain.
            for (var levelIndex = 0;; levelIndex++)
            {
                // Init level.
                chunkStoresWithReferences.Clear();
                fetchAndRecoverTasks.Clear();
                referenceDecodedChunkMap.Clear();
                
                // Optimize value search for chunks in level.
                // Analyzing from right, it can only be monotonically non-decreasing.
                long dataSizeBySegment = SwarmCac.DataSize;

                // Parse all chunks on current level. Start from end to copy data first, if present in level.
                for (int chunkIndex = levelDecodedChunks.Length - 1; chunkIndex >= 0; chunkIndex--)
                {
                    // Get decoded chunk info.
                    var (reference, redundancyLevel, parities, _, chunkData) = levelDecodedChunks[chunkIndex];
                    var spanLength = (long)levelDecodedChunks[chunkIndex].SpanLength;

                    var isFirstChunkInLevel = chunkIndex == 0;
                    var isLastChunkInLevel = chunkIndex == levelDecodedChunks.Length - 1;

                    // If it's the last chunk on the level, and if some data remains to read in level, cache the chunk.
                    if (isLastChunkInLevel && levelEndDataOffset > 0)
                        newLevelsCache.Add(levelDecodedChunks[chunkIndex]);

                    // If is a data chunk, report data on buffer and update bounds. Then continue.
                    if (spanLength <= SwarmCac.DataSize)
                    {
                        //check end offset consistency
                        var dataToCopyStart = isFirstChunkInLevel ? (int)levelStartDataOffset : 0;
                        var dataToCopySize = chunkData.Length - dataToCopyStart - (int)levelEndDataOffset;
                        if (dataToCopySize <= 0)
                            throw new InvalidOperationException("Invalid data to copy size");

                        //copy data to end of buffer, and shrink buffer
                        chunkData[dataToCopyStart..(dataToCopyStart + dataToCopySize)].CopyTo(buffer[^dataToCopySize..]);
                        buffer = buffer[..^dataToCopySize];

                        //update level bounds
                        levelEndDataOffset = 0;

                        continue;
                    }

                    // Extract references.
                    var references = SwarmCac.GetIntermediateReferencesFromData(
                        chunkData.Span, parities, reference.IsEncrypted);

                    // Find referred data size by data reference.
                    var dataReferencesAmount = references.Count(r => !r.IsParity);
                    while (dataSizeBySegment * dataReferencesAmount < spanLength)
                        dataSizeBySegment *= maxDataSegmentsInChunk;

                    // Define chunk's data references to read and set bounds for the next level.
                    var startDataReferenceToSkip = 0;
                    var endDataReferencesToSkip = 0;
                    if (isFirstChunkInLevel)
                    {
                        startDataReferenceToSkip = (int)(levelStartDataOffset / dataSizeBySegment);
                        levelStartDataOffset -= startDataReferenceToSkip * dataSizeBySegment;
                    }
                    if (isLastChunkInLevel)
                    {
                        var lastPartialSegmentDataSize = spanLength % dataSizeBySegment;
                        if (levelEndDataOffset >= lastPartialSegmentDataSize)
                        {
                            endDataReferencesToSkip = (int)((levelEndDataOffset - lastPartialSegmentDataSize) / dataSizeBySegment);
                            if (lastPartialSegmentDataSize > 0)
                                endDataReferencesToSkip++;
                        }
                        if (endDataReferencesToSkip > 0)
                            levelEndDataOffset -= spanLength % dataSizeBySegment == 0
                                ? endDataReferencesToSkip * dataSizeBySegment
                                : (endDataReferencesToSkip - 1) * dataSizeBySegment + spanLength % dataSizeBySegment;
                    }

                    // Fetch and try to recover child chunks with parities.
                    var decoder = new ChunkParityDecoder(references, chunkStore);

                    chunkStoresWithReferences.Insert(0,
                        (decoder, references
                            .TakeWhile(r => !r.IsParity)
                            .Select(r => r.Reference)
                            .Skip(startDataReferenceToSkip)
                            .SkipLast(endDataReferencesToSkip)
                            .ToDictionary(r => r.Hash, r => r)));

                    // Run fetch and recover asynchronously.
                    fetchAndRecoverTasks.Add(decoder.FetchAndRecoverAsync(
                        redundancyStrategy,
                        redundancyStrategyFallback,
                        cancellationToken: cancellationToken));
                }

                // Wait all "fetch and recover" tasks for this level, when "redundancyLevel != RedundancyLevel.None".
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

                // Start get decoded chunks tasks for the next level.
                foreach (var (chunkStore, hashReferenceMap) in chunkStoresWithReferences)
                {
                    // Get and decode chunks from store.
                    var decodedChunks = hashReferenceMap.Values
                        .Where(r => levelsCache.Count <= levelIndex + 1 ||
                                    r != levelsCache[levelIndex + 1].Reference)
                        .Select(r => chunkStore.GetChunk(r.Hash).Decode(hashReferenceMap[r.Hash], hasher));

                    // Add decoded chunk from cache.
                    if (levelsCache.Count > levelIndex + 1 &&
                        hashReferenceMap.ContainsKey(levelsCache[levelIndex + 1].Reference.Hash))
                        decodedChunks = decodedChunks.Append(levelsCache[levelIndex + 1]);
                    
                    foreach (var decodedChunk in decodedChunks)
                        referenceDecodedChunkMap.Add(decodedChunk.Reference, decodedChunk);
                }

                // Set new level decoded chunks in order.
                levelDecodedChunks = chunkStoresWithReferences.SelectMany(p => p.HashReferenceMap.Values)
                    .Select(r => referenceDecodedChunkMap[r]).ToArray();
            }
        }
    }
}