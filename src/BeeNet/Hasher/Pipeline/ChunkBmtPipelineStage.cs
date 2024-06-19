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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Pipeline
{
    /// <summary>
    /// Calculate hash of each chunk
    /// </summary>
    internal sealed class ChunkBmtPipelineStage : IHasherPipelineStage
    {
        // Consts.
        public const int MinCompactLevel = 0;
        public const int MaxCompactLevel = 100;

        // Fields.
        private readonly double compactTriggerMultiplicator;
        private readonly IHasherPipelineStage nextStage;
        private readonly IPostageStampIssuer stampIssuer;
        private readonly bool useCompaction;

        // Constructor.
        public ChunkBmtPipelineStage(
            int compactionLevel,
            IHasherPipelineStage nextStage,
            IPostageStampIssuer stampIssuer)
        {
            if (compactionLevel is < MinCompactLevel or > MaxCompactLevel)
                throw new ArgumentOutOfRangeException(nameof(compactionLevel));

            this.nextStage = nextStage;
            this.stampIssuer = stampIssuer;

            //precalculate compaction trigger level multiplicator
            useCompaction = compactionLevel != 0;
            if (useCompaction)
            {
                compactTriggerMultiplicator = (
                    (PostageBuckets.BucketsSize - 1) * Math.Pow(compactionLevel, 2) / Math.Pow(MaxCompactLevel, 2) -
                    (PostageBuckets.BucketsSize - 1) * 2.0 * compactionLevel / MaxCompactLevel +
                    PostageBuckets.BucketsSize) / PostageBuckets.BucketsSize;
            }
        }

        // Dispose.
        public void Dispose()
        {
            nextStage.Dispose();
        }

        // Methods.
        public async Task FeedAsync(HasherPipelineFeedArgs args)
        {
            if (args.Data.Length < SwarmChunk.SpanSize)
                throw new InvalidOperationException("Data can't be shorter than span size here");
            if (args.Data.Length > SwarmChunk.SpanAndDataSize)
                throw new InvalidOperationException("Data can't be longer than chunk + span size here");

            if (!useCompaction)
            {
                // Get simply the chunk hash.
                args.Hash = SwarmChunkBmtHasher.Hash(
                    args.Data[..SwarmChunk.SpanSize].ToArray(),
                    args.Data[SwarmChunk.SpanSize..].ToArray());
            }
            else
            {
                var span = args.Data[..SwarmChunk.SpanSize].ToArray();
                var data = args.Data[SwarmChunk.SpanSize..].ToArray();
                
                // Try to mine a valid seed value.
                SwarmHash chunkHash = default;
                uint bucketId = default;
                for (int i = 0; i < byte.MaxValue; i++)
                {
                    chunkHash = SwarmChunkBmtHasher.Hash(span, data);
                    bucketId = chunkHash.ToBucketId();
                    
                    var collisions = stampIssuer.GetCollisions(bucketId);
                    var triggerLevel = GetCompactionTrigger(stampIssuer.TotalChunks);
                    
                    if (collisions < triggerLevel)
                        break;

                    if (i + 1 < byte.MaxValue)
                        data[0]++;
                }

                args.Hash = chunkHash;
            }

            await nextStage.FeedAsync(args).ConfigureAwait(false);
        }

        public Task<SwarmHash> SumAsync() => nextStage.SumAsync();
        
        // Helpers.
        private uint GetCompactionTrigger(long totalChunkCount) =>
            (uint)Math.Floor(totalChunkCount * compactTriggerMultiplicator) + 1;
    }
}