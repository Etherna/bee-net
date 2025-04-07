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

using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmSequenceFeed(EthAddress owner, SwarmFeedTopic topic)
        : SwarmFeedBase(owner, topic)
    {
        // Consts.
        private const int DefaultSearchLevels = 8;
        
        // Fields.
        private readonly ConcurrentQueue<IHasher> hasherPool = new();

        // Properties.
        public override SwarmFeedType Type => SwarmFeedType.Sequence;

        // Methods.
        public override async Task<SwarmFeedChunkBase> BuildNextFeedChunkAsync(
            ReadOnlyMemory<byte> contentData,
            SwarmFeedIndexBase? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            Func<ISwarmChunkBmt> bmtBuilder,
            DateTimeOffset? timestamp = null)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            
            return await BuildNextFeedChunkAsync(
                contentData,
                knownNearIndex as SwarmSequenceFeedIndex,
                chunkStore,
                bmtBuilder).ConfigureAwait(false);
        }

        public async Task<SwarmSequenceFeedChunk> BuildNextFeedChunkAsync(
            ReadOnlyMemory<byte> contentData,
            SwarmSequenceFeedIndex? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            Func<ISwarmChunkBmt> bmtBuilder)
        {
            ArgumentNullException.ThrowIfNull(bmtBuilder, nameof(bmtBuilder));
            
            // Find last published chunk.
            var lastFeedChunk = await TryFindLastFeedChunkAsync(
                knownNearIndex, chunkStore, () => bmtBuilder().Hasher).ConfigureAwait(false);

            // Define next sequence index.
            var nextSequenceIndex = lastFeedChunk is null ?
                new SwarmSequenceFeedIndex(0) :
                (SwarmSequenceFeedIndex)lastFeedChunk.Index.GetNext(0);

            // Create new chunk.
            var swarmChunkBmt = bmtBuilder();
            return new SwarmSequenceFeedChunk(
                Topic,
                nextSequenceIndex,
                BuildIdentifier(nextSequenceIndex, swarmChunkBmt.Hasher),
                Owner,
                SwarmSequenceFeedChunk.BuildInnerChunk(contentData, swarmChunkBmt),
                null);
        }

        public override async Task<SwarmFeedChunkBase?> TryFindFeedChunkAtAsync(
            long at,
            SwarmFeedIndexBase? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            Func<IHasher> hasherBuilder)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            
            return await TryFindLastFeedChunkAsync(
                knownNearIndex as SwarmSequenceFeedIndex,
                chunkStore,
                hasherBuilder).ConfigureAwait(false);
        }

        public async Task<SwarmSequenceFeedChunk?> TryFindLastFeedChunkAsync(
            SwarmSequenceFeedIndex? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            Func<IHasher> hasherBuilder,
            TimeSpan? requestsCustomTimeout = null)
        {
            ArgumentNullException.ThrowIfNull(hasherBuilder, nameof(hasherBuilder));
            
            // First lookup at the knownNearIndex, or default index(0).
            var startIndex = knownNearIndex ?? new SwarmSequenceFeedIndex(0);
            var chunk = await TryGetFeedChunkAsync(startIndex, chunkStore, hasherBuilder()).ConfigureAwait(false);
            if (chunk is not SwarmSequenceFeedChunk sequenceFeedChunk)
                return null;

            // If chunk exists, start a recursive concurrent lookup.
            return await RunLookupsAsync(
                chunkStore,
                DefaultSearchLevels,
                sequenceFeedChunk,
                hasherBuilder,
                requestsCustomTimeout).ConfigureAwait(false);
        }

        // Helpers.
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
        private async Task<SwarmSequenceFeedChunk?> RunLookupsAsync(
            IReadOnlyChunkStore chunkStore,
            int maxSearchLevel,
            SwarmSequenceFeedChunk bestFoundChunk,
            Func<IHasher> hasherBuilder,
            TimeSpan? requestsCustomTimeout = null)
        {
            using var semaphore = new SemaphoreSlim(1, 1);
            var tasks = new List<Task>();

            SwarmSequenceFeedIndex baseIndex = (SwarmSequenceFeedIndex)bestFoundChunk.Index;
            int bestFoundLevel = 0;
            List<int> notFoundLevels = [DefaultSearchLevels + 1];
            SwarmSequenceFeedChunk? feedChunkResult = null;
            
            for (var l = 1; l <= maxSearchLevel; l++)
            {
                var level = l;
                tasks.Add(Task.Run(async () =>
                {
                    // Init.
                    if (!hasherPool.TryDequeue(out var hasher))
                        hasher = hasherBuilder();
                    using var timeoutCancellationTokenSource = new CancellationTokenSource(requestsCustomTimeout ?? DefaultTimeout);
                    
                    // Exec lookup.
                    var index = new SwarmSequenceFeedIndex(baseIndex.Value + ((ulong)1 << level) - 1);
                    var chunk = await TryGetFeedChunkAsync(
                        index,
                        chunkStore,
                        hasher,
                        timeoutCancellationTokenSource.Token).ConfigureAwait(false)
                        as SwarmSequenceFeedChunk;
                    
                    // Evaluate result. Use semaphore on evaluation because of concurrent requests.
                    try //catch timeout exception
                    {
                        await semaphore.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            //if bestFoundLevel is higher than current level, this result can be skipped
                            if (level < bestFoundLevel)
                                return;
                            
                            //try update edge results
                            if (chunk == null)
                            {
                                //keep trace to try recover in case this could be a lookup error
                                notFoundLevels.Add(level);
                                
                                //skip if level can't lower not found minimum level
                                if (notFoundLevels.Min() < level)
                                    return;
                            }
                            else
                            {
                                //report best result
                                bestFoundChunk = chunk;
                                bestFoundLevel = level;
                                
                                //adjust not found levels. If any previous result have failed to lookup
                                //for an existing chunk, remove wrong levels
                                notFoundLevels.RemoveAll(nfl => nfl < bestFoundLevel);
                            }
                            
                            // Check ending/recursion conditions.
                            
                            //if a chunk is found on the max level, and this is already a sub-interval,
                            //then index+1 is already known to be not found
                            if (chunk != null &&
                                level == maxSearchLevel &&
                                maxSearchLevel < DefaultSearchLevels)
                            {
                                feedChunkResult = chunk;
                                return;
                            }
                            
                            //if current interval is completed
                            if (bestFoundLevel + 1 == notFoundLevels.Min())
                            {
                                //if best found result was from previous recursion (level == 0)
                                if (bestFoundLevel == 0)
                                {
                                    feedChunkResult = bestFoundChunk;
                                    return;
                                }

                                //else, go more in deep with better interval
                                feedChunkResult = await RunLookupsAsync(
                                    chunkStore: chunkStore,
                                    maxSearchLevel: bestFoundLevel,
                                    bestFoundChunk: bestFoundChunk,
                                    hasherBuilder).ConfigureAwait(false);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                            hasherPool.Enqueue(hasher);
                        }
                    }
                    catch (OperationCanceledException) { }
                }));
            }
            
            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);

            return feedChunkResult;
        }
    }
}
