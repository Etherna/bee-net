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

using Etherna.BeeNet.Hashing.Bmt;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    internal sealed class ChunkAggregatorPipelineStage(
        ChunkBmtPipelineStage shortBmtPipelineStage,
        bool useRecursiveEncryption)
        : IHasherPipelineStage
    {
        // Private classes.
        private sealed class ChunkHeader(
            SwarmHash hash,
            ReadOnlyMemory<byte> span,
            XorEncryptKey? chunkKey,
            bool isParityChunk)
        {
            public XorEncryptKey? ChunkKey { get; } = chunkKey;
            public SwarmHash Hash { get; } = hash;
            public ReadOnlyMemory<byte> Span { get; } = span;
            public bool IsParityChunk { get; } = isParityChunk;
        }
        
        // Fields.
        private readonly SemaphoreSlim feedChunkMutex = new(1, 1);
        private readonly Dictionary<long, HasherPipelineFeedArgs> feedingBuffer = new();
        private readonly List<List<ChunkHeader>> chunkLevels = []; //[level][chunk]
        private readonly byte maxChildrenChunks = (byte)(useRecursiveEncryption
            ? SwarmChunkBmt.SegmentsCount / 2 //write chunk key after chunk hash
            : SwarmChunkBmt.SegmentsCount);

        private long feededChunkNumberId;

        // Dispose.
        public void Dispose()
        {
            feedChunkMutex.Dispose();
        }
        
        // Properties.
        public long MissedOptimisticHashing => shortBmtPipelineStage.MissedOptimisticHashing;
        
        // Methods.
        public async Task FeedAsync(HasherPipelineFeedArgs args)
        {
            await feedChunkMutex.WaitAsync().ConfigureAwait(false);
            try
            {
                // Catch all the chunks from async tasks.
                // Returns only the next sequential arrived chunks.
                feedingBuffer.Add(args.NumberId, args);

                List<HasherPipelineFeedArgs> chunksToProcess = [];
                while (feedingBuffer.Remove(feededChunkNumberId, out var nextChunk))
                {
                    chunksToProcess.Add(nextChunk);
                    feededChunkNumberId++;
                }
                
                // Process all the ready sequential chunks.
                foreach (var processingChunk in chunksToProcess)
                {
                    await AddChunkToLevelAsync(
                        1,
                        new ChunkHeader(
                            processingChunk.Hash!.Value,
                            processingChunk.Span,
                            processingChunk.ChunkKey,
                            false)).ConfigureAwait(false);
                }
            }
            finally
            {
                feedChunkMutex.Release();
            }
        }

        public async Task<SwarmHashTree> SumAsync()
        {
            bool rootChunkFound = false;
            for (int i = 0; !rootChunkFound; i++)
            {
                var levelChunks = GetLevelChunks(i);
                var isLastLevel = i == chunkLevels.Count - 1;
                switch (levelChunks.Count)
                {
                    case 0:
                        if (isLastLevel)
                            throw new InvalidOperationException("Can't be last level with 0 chunks");
                        break; //level empty, continue to the next
                    
                    case 1:
                        rootChunkFound = isLastLevel;
                        break;
                    
                    default:
                        await WrapFullLevelAsync(i).ConfigureAwait(false);
                        break;
                }
            }
            
            var rootChunk = chunkLevels.Last()[0];

            return new SwarmHashTree(
                new(rootChunk.Hash, rootChunk.ChunkKey, useRecursiveEncryption),
                [/*TODO*/]);
        }

        // Helpers.
        private async Task AddChunkToLevelAsync(int level, ChunkHeader chunkHeader)
        {
            ArgumentNullException.ThrowIfNull(chunkHeader, nameof(chunkHeader));

            var levelChunks = GetLevelChunks(level);
            levelChunks.Add(chunkHeader);
            
            if (levelChunks.Count == maxChildrenChunks)
                await WrapFullLevelAsync(level).ConfigureAwait(false);
        }

        private List<ChunkHeader> GetLevelChunks(int level)
        {
            while (chunkLevels.Count < level + 1)
                chunkLevels.Add([]);
            return chunkLevels[level];
        }
        
        private async Task WrapFullLevelAsync(int level)
        {
            var levelChunks = GetLevelChunks(level);

            // Calculate total span of all not parity chunks in level.
            var totalSpan = SwarmChunk.LengthToSpan(
                levelChunks.Where(c => !c.IsParityChunk) //don't add span of parity chunks to the common
                    .Select(c => SwarmChunk.SpanToLength(c.Span.Span))
                    .Aggregate((a,c) => a + c)); //sum of ulongs. Linq doesn't have it
            
            // Build total data from total span, and all the hashes in level.
            // If chunks are compacted, append the encryption key after the chunk hash.
            var totalData = totalSpan.Concat(
                levelChunks.SelectMany(c => useRecursiveEncryption
                    ? c.Hash.ToByteArray().Concat(c.ChunkKey!.Bytes.ToArray())
                    : c.Hash.ToByteArray()))
                .ToArray();

            // Run hashing on the new chunk, and add it to next level.
            var hashingResult = await HashIntermediateChunkAsync(totalSpan, totalData).ConfigureAwait(false);
            await AddChunkToLevelAsync(
                level + 1,
                new ChunkHeader(
                    hashingResult.Hash,
                    totalSpan,
                    hashingResult.EncryptionKey,
                    false)).ConfigureAwait(false);
            
            levelChunks.Clear();
        }
        
        // Helpers.
        private async Task<SwarmChunkReference> HashIntermediateChunkAsync(byte[] span, byte[] data)
        {
            var args = new HasherPipelineFeedArgs(span: span, data: data);
            await shortBmtPipelineStage.FeedAsync(args).ConfigureAwait(false);
            return new(args.Hash!.Value, args.ChunkKey, useRecursiveEncryption);
        }
    }
}