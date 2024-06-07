// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Hasher.Bmt;
using Etherna.BeeNet.Hasher.Postage;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Pipeline
{
    internal delegate Task<SwarmAddress> HashChunkDelegateAsync(byte[] span, byte[] data);
    
    internal sealed class ChunkAggregatorPipelineStage : IHasherPipelineStage
    {
        // Private classes.
        private class ChunkHeader(SwarmAddress address, ReadOnlyMemory<byte> span, bool isParityChunk)
        {
            public SwarmAddress Address { get; } = address;
            public ReadOnlyMemory<byte> Span { get; } = span;
            public bool IsParityChunk { get; } = isParityChunk;
        }
        
        // Fields.
        private readonly SemaphoreSlim feedChunkMutex = new(1, 1);
        private readonly Dictionary<long, HasherPipelineFeedArgs> feedingBuffer = new();
        private readonly List<List<ChunkHeader>> chunkLevels; //[level][chunk]
        private readonly HashChunkDelegateAsync hashChunkDelegate;
        private readonly byte maxChildrenChunks;
        
        private long feededChunkNumberId;

        // Constructor.
        public ChunkAggregatorPipelineStage(
            HashChunkDelegateAsync hashChunkDelegate)
        {
            chunkLevels = [];
            this.hashChunkDelegate = hashChunkDelegate;
            maxChildrenChunks = SwarmChunkBmt.SegmentsCount;
        }
        
        // Dispose.
        public void Dispose()
        {
            feedChunkMutex.Dispose();
        }
        
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
                            processingChunk.Address!.Value,
                            processingChunk.Span,
                            false)).ConfigureAwait(false);
                }
            }
            finally
            {
                feedChunkMutex.Release();
            }
        }

        public async Task<SwarmAddress> SumAsync()
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

            return rootChunk.Address;
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
            
            // Build total data from total span, and all the addresses in level.
            var totalData = totalSpan.Concat(
                levelChunks.SelectMany(c => c.Address.ToByteArray()))
                .ToArray();

            // Run hashing on the new chunk, and add it to next level.
            await AddChunkToLevelAsync(
                level + 1,
                new ChunkHeader(
                    await hashChunkDelegate(totalSpan, totalData).ConfigureAwait(false),
                    totalSpan,
                    false)).ConfigureAwait(false);
            
            levelChunks.Clear();
        }
    }
}