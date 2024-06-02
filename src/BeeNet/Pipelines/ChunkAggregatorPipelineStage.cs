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

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Postage;
using Etherna.BeeNet.Redundancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    public delegate Task<SwarmAddress> HashChunkDelegateAsync(byte[] span, byte[] data);
    
    internal sealed class ChunkAggregatorPipelineStage : PipelineStageBase, IDisposable
    {
        // Private classes.
        private class ChunkHeader(SwarmAddress address, ReadOnlyMemory<byte> span, bool isParityChunk, byte[]? key)
        {
            public SwarmAddress Address { get; } = address;
            public ReadOnlyMemory<byte> Span { get; } = span;
            public bool IsParityChunk { get; } = isParityChunk;
            public byte[] Key { get; } = key ?? [];
        }
        
        // Fields.
        private readonly AddParityChunkCallback addParityChunkFunc;
        private readonly SemaphoreSlim feedChunkMutex = new(1, 1);
        private readonly Dictionary<long, PipelineFeedArgs> feedingBuffer = new();
        private readonly List<List<ChunkHeader>> chunkLevels; //[level][chunk]
        private readonly RedundancyParams redundancyParams;
        private readonly HashChunkDelegateAsync hashChunkDelegate;
        private readonly byte maxChildrenChunks;
        
        private long feededChunkNumberId;
        
        /// <summary>
        /// Putter to save dispersed replicas of the root chunk
        /// </summary>
        private readonly ReplicaPutter replicaPutter;

        // Constructor.
        public ChunkAggregatorPipelineStage(
            RedundancyParams redundancyParams,
            IPostageStamper postageStamper,
            HashChunkDelegateAsync hashChunkDelegate)
            : base(null)
        {
            addParityChunkFunc = (level, span, address) => AddChunkToLevelAsync(level, new ChunkHeader(address, span, true, null));
            chunkLevels = [];
            this.hashChunkDelegate = hashChunkDelegate;
            maxChildrenChunks = (byte)(redundancyParams.MaxShards + redundancyParams.Parities(redundancyParams.MaxShards));
            this.redundancyParams = redundancyParams ?? throw new ArgumentNullException(nameof(redundancyParams));
            replicaPutter = new ReplicaPutter(postageStamper, redundancyParams.Level);
        }
        
        // Dispose.
        public override void Dispose()
        {
            feedChunkMutex.Dispose();
            base.Dispose();
        }

        // Protected methods.
        protected override async Task FeedImplAsync(PipelineFeedArgs args)
        {
            await feedChunkMutex.WaitAsync().ConfigureAwait(false);
            try
            {
                // Catch all the chunks from async tasks.
                // Returns only the next sequential arrived chunks.
                feedingBuffer.Add(args.NumberId, args);

                List<PipelineFeedArgs> chunksToProcess = [];
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
                            false,
                            processingChunk.EncryptionKey)).ConfigureAwait(false);

                    if (redundancyParams.Level != RedundancyLevel.None)
                        await redundancyParams.ChunkWriteAsync(
                            0,
                            processingChunk.Data.ToArray(),
                            addParityChunkFunc).ConfigureAwait(false);
                }
            }
            finally
            {
                feedChunkMutex.Release();
            }
        }

        protected override async Task<SwarmAddress> SumImplAsync()
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
                        
                        //replace cached chunk to the level as well
                        await redundancyParams.ElevateCarrierChunkAsync(i - 1, addParityChunkFunc).ConfigureAwait(false);
                        break;
                    
                    default:
                        if (levelChunks.Count != maxChildrenChunks)
                        {
                            // call erasure encoding before writing the last chunk on the level
                            await redundancyParams.EncodeAsync(i - 1, addParityChunkFunc).ConfigureAwait(false);
                        }

                        await WrapFullLevelAsync(i).ConfigureAwait(false);
                        break;
                }
            }
            
            var rootChunk = chunkLevels.Last()[0];

	        // save disperse replicas of the root chunk
	        if (redundancyParams.Level != RedundancyLevel.None)
            {
                var rootData = redundancyParams.GetRootData();
                replicaPutter.Put(SwarmChunk.BuildFromSpanAndData(rootChunk.Address, rootData));
	        }

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
            
            if (levelChunks.Any(c => c.IsParityChunk))
            {
                // EncodeLevel encodes used redundancy level for uploading into span keeping the real byte count for the chunk.
                // assumes span is LittleEndian.
                // set parity in the most signifact byte
                totalSpan[SwarmChunk.SpanSize - 1] = (byte)((int)redundancyParams.Level | (1 << 7)); // p + 128
            }
            
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
                    false,
                    null)).ConfigureAwait(false);
            
            await redundancyParams.ChunkWriteAsync(level, totalData, addParityChunkFunc).ConfigureAwait(false);
            
            levelChunks.Clear();
        }
    }
}