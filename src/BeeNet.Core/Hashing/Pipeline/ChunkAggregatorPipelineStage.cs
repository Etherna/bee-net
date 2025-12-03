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

using Etherna.BeeNet.Chunks;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    internal sealed class ChunkAggregatorPipelineStage(
        ChunkParityGenerator parityGenerator,
        ChunkReplicator chunkReplicator,
        ChunkBmtPipelineStage shortBmtPipelineStage,
        bool readOnly)
        : IHasherPipelineStage
    {
        // Fields.
        private readonly SemaphoreSlim feedChunkMutex = new(1, 1);
        private readonly Dictionary<long, HasherPipelineFeedArgs> feedingBuffer = new();
        private readonly List<List<SwarmChunkHeader>> chunkLevels = []; //[level][chunk]
        
        private long feededChunkNumberId;
        
        // Dispose.
        public void Dispose()
        {
            feedChunkMutex.Dispose();
        }
        
        // Properties.
        public long MissedOptimisticHashing => shortBmtPipelineStage.MissedOptimisticHashing;
        public IPostageStamper PostageStamper => shortBmtPipelineStage.PostageStamper;

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
                        0,
                        new SwarmChunkHeader(
                            processingChunk.Reference!.Value,
                            processingChunk.Span,
                            false),
                        args.SwarmChunkBmt).ConfigureAwait(false);
                    
                    await parityGenerator.AddChunkToLevelAsync(
                        0,
                        processingChunk.SpanData,
                        AddChunkToLevelAsync,
                        args.SwarmChunkBmt).ConfigureAwait(false);
                }
            }
            finally
            {
                feedChunkMutex.Release();
            }
        }

        public async Task<SwarmReference> SumAsync(SwarmChunkBmt swarmChunkBmt)
        {
            var rootChunkFound = false;
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
                        if (isLastLevel)
                            rootChunkFound = true;
                        else //carry on current chunk on next level
                        {
                            var nextLevelChunks = GetLevelChunks(i + 1);
                            nextLevelChunks.Add(levelChunks.Single());
                            levelChunks.Clear();
                            
                            await parityGenerator.ElevateCarrierChunkAsync(
                                i, AddChunkToLevelAsync, swarmChunkBmt).ConfigureAwait(false);
                        }
                        break;
                    
                    default:
                        await parityGenerator.EncodeErasureDataAsync(
                            i, AddChunkToLevelAsync, swarmChunkBmt).ConfigureAwait(false);
                        await WrapFullLevelAsync(i, swarmChunkBmt).ConfigureAwait(false);
                        break;
                }
            }
            
            var rootChunk = chunkLevels.Last()[0];
            
            // Create disperse replicas of root chunk. Don't add if is readOnly.
            if (parityGenerator.RedundancyLevel != RedundancyLevel.None && !readOnly)
            {
                var rootSpanData = parityGenerator.GetRootSpanData();
                await chunkReplicator.AddChunkReplicasAsync(
                    new SwarmCac(rootChunk.Reference.Hash, rootSpanData),
                    swarmChunkBmt.Hasher).ConfigureAwait(false);
            }

            return rootChunk.Reference;
        }

        // Helpers.
        private async Task AddChunkToLevelAsync(int level, SwarmChunkHeader chunkHeader, SwarmChunkBmt swarmChunkBmt)
        {
            ArgumentNullException.ThrowIfNull(chunkHeader, nameof(chunkHeader));

            var levelChunks = GetLevelChunks(level);
            levelChunks.Add(chunkHeader);
            
            if (levelChunks.Count == parityGenerator.MaxChildrenChunks)
                await WrapFullLevelAsync(level, swarmChunkBmt).ConfigureAwait(false);
        }

        private List<SwarmChunkHeader> GetLevelChunks(int level)
        {
            while (chunkLevels.Count < level + 1)
                chunkLevels.Add([]);
            return chunkLevels[level];
        }
        
        private async Task<HasherPipelineFeedArgs> HashIntermediateChunkAsync(
            ReadOnlyMemory<byte> span,
            ReadOnlyMemory<byte> spanData,
            SwarmChunkBmt swarmChunkBmt)
        {
            var args = new HasherPipelineFeedArgs(swarmChunkBmt: swarmChunkBmt, span: span, spanData: spanData);
            await shortBmtPipelineStage.FeedAsync(args).ConfigureAwait(false);
            return args;
        }
        
        private async Task WrapFullLevelAsync(int level, SwarmChunkBmt swarmChunkBmt)
        {
            var levelChunks = GetLevelChunks(level);

            // Calculate total span of all not parity chunks in level.
            var totalSpan = SwarmCac.LengthToSpan(
                levelChunks.Where(c => !c.IsParityChunk) //don't add span of parity chunks to the common
                    .Select(c => SwarmCac.DecodedSpanToLength(c.Span.Span))
                    .Aggregate((a,c) => a + c)); //sum of ulongs. Linq doesn't have it
            if (levelChunks.Any(c => c.IsParityChunk))
                SwarmCac.EncodeSpan(totalSpan, parityGenerator.RedundancyLevel);
            
            // Build total data from total span, and all the hashes in level.
            // If chunks are encrypted, append the encryption key after the chunk hash.
            var dataChunksInLevel = levelChunks.Count(c => !c.IsParityChunk);
            var parityChunksInLevel = levelChunks.Count - dataChunksInLevel;
            var totalDataLength = SwarmCac.SpanSize +
                dataChunksInLevel * (parityGenerator.EncryptChunks ? SwarmReference.EncryptedSize : SwarmReference.PlainSize) +
                parityChunksInLevel * SwarmReference.PlainSize; //parity references only have hashes
            var totalSpanData = new byte[totalDataLength];
            var totalDataIndex = 0;
            
            totalSpan.CopyTo(totalSpanData, totalDataIndex);
            totalDataIndex += SwarmCac.SpanSize;
            foreach (var chunk in levelChunks)
            {
                chunk.Reference.ToReadOnlyMemory().CopyTo(totalSpanData.AsMemory()[totalDataIndex..]);
                totalDataIndex += chunk.Reference.Size;
            }

            // Run hashing on the new chunk, and add it to next level.
            var intermediateHashingArgs = await HashIntermediateChunkAsync(
                totalSpan, totalSpanData, swarmChunkBmt).ConfigureAwait(false);
            await AddChunkToLevelAsync(
                level + 1,
                new SwarmChunkHeader(
                    intermediateHashingArgs.Reference!.Value,
                    totalSpan,
                    false),
                swarmChunkBmt).ConfigureAwait(false);
            
            levelChunks.Clear();

            // Add chunk to redundancy generator.
            await parityGenerator.AddChunkToLevelAsync(
                level + 1,
                intermediateHashingArgs.SpanData,
                AddChunkToLevelAsync,
                swarmChunkBmt).ConfigureAwait(false);
        }
    }
}