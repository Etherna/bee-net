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

using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class SwarmSequenceFeed(EthAddress owner, byte[] topic)
        : SwarmFeedBase(owner, topic)
    {
        // Consts.
        private const int DefaultSearchLevels = 8;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        // Properties.
        public override SwarmFeedType Type => SwarmFeedType.Sequence;

        // Methods.
        public override Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            byte[] contentPayload,
            SwarmFeedIndexBase? knownNearIndex)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            return BuildNextFeedChunkAsync(chunkStore, contentPayload, knownNearIndex as SwarmSequenceFeedIndex);
        }

        public Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            byte[] contentPayload,
            SwarmSequenceFeedIndex? knownNearIndex)
        {
            throw new NotImplementedException();
        }

        public override Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            long at,
            SwarmFeedIndexBase? knownNearIndex)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            return TryFindFeedAtAsync(chunkStore, knownNearIndex as SwarmSequenceFeedIndex);
        }

        public async Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            SwarmSequenceFeedIndex? knownNearIndex,
            TimeSpan? requestsCustomTimeout = null)
        {
            // First lookup at the knownNearIndex, or default index(0).
            var startIndex = knownNearIndex ?? new SwarmSequenceFeedIndex(0);
            var chunk = await TryGetFeedChunkAsync(chunkStore, startIndex).ConfigureAwait(false);
            if (chunk == null)
                return null;

            // If chunk exists, start a recursive concurrent lookup.
            return await RunLookupsAsync(
                chunkStore: chunkStore,
                maxSearchLevel: DefaultSearchLevels,
                bestFoundChunk: chunk,
                requestsCustomTimeout: requestsCustomTimeout).ConfigureAwait(false);
        }

        // Helpers.
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
        private async Task<SwarmFeedChunk?> RunLookupsAsync(
            IReadOnlyChunkStore chunkStore,
            int maxSearchLevel,
            SwarmFeedChunk bestFoundChunk,
            TimeSpan? requestsCustomTimeout = null)
        {
            using var semaphore = new SemaphoreSlim(1, 1);
            var tasks = new List<Task>();

            SwarmSequenceFeedIndex baseIndex = (SwarmSequenceFeedIndex)bestFoundChunk.Index;
            int bestFoundLevel = 0;
            List<int> notFoundLevels = [DefaultSearchLevels + 1];
            SwarmFeedChunk? feedChunkResult = null;
            
            for (var l = 1; l <= maxSearchLevel; l++)
            {
                var level = l;
                tasks.Add(Task.Run(async () =>
                {
                    // Exec lookup.
                    using var timeoutCancellationTokenSource = new CancellationTokenSource(requestsCustomTimeout ?? DefaultTimeout);

                    var index = new SwarmSequenceFeedIndex(baseIndex.Value + ((ulong)1 << level) - 1);
                    var chunk = await TryGetFeedChunkAsync(
                        chunkStore,
                        index,
                        timeoutCancellationTokenSource.Token).ConfigureAwait(false);
                    
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
                                    bestFoundChunk: bestFoundChunk).ConfigureAwait(false);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
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
