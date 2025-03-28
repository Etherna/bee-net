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

        // Private classes.
        private class ChunkGetResult
        {
            public required SwarmChunk? Chunk { get; init; }
            public int Level { get; set; }
            public required SwarmSequenceFeedIndex Index { get; init; }
        }

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
                baseIndex: startIndex,
                minSearchLevel: 0,
                maxSearchLevel: DefaultSearchLevels,
                bestFoundResult: new ChunkGetResult
                {
                    Chunk = chunk,
                    Index = startIndex,
                    Level = 0
                },
                lowestNotFoundLevel: DefaultSearchLevels + 1,
                requestsCustomTimeout: requestsCustomTimeout).ConfigureAwait(false);
        }

        // Helpers.
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        private async Task<SwarmFeedChunk?> RunLookupsAsync(
            IReadOnlyChunkStore chunkStore,
            SwarmSequenceFeedIndex baseIndex,
            int minSearchLevel,
            int maxSearchLevel,
            ChunkGetResult bestFoundResult,
            int lowestNotFoundLevel,
            TimeSpan? requestsCustomTimeout = null)
        {
            using var semaphore = new SemaphoreSlim(1, 1);
            var tasks = new List<Task>();
            SwarmFeedChunk? feedChunkResult = null;
            
            for (var l = minSearchLevel; l <= maxSearchLevel; l++)
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
                        await semaphore.WaitAsync(timeoutCancellationTokenSource.Token).ConfigureAwait(false);
                        try
                        {
                            //if a result is found, terminate other parallel evaluations
                            if (feedChunkResult != null)
                                return;
                            
                            //skip results external to current best edges
                            if ((chunk == null && lowestNotFoundLevel < level) ||    //can't lower not found level
                                (chunk != null && level < bestFoundResult.Level))    //can't higher found level
                                return;

                            //update edge results
                            if (chunk == null)
                                lowestNotFoundLevel = level;
                            else
                                bestFoundResult = new ChunkGetResult
                                {
                                    Chunk = chunk,
                                    Index = index,
                                    Level = level,
                                };
                            
                            // Check ending/recursion conditions.
                            
                            //if a chunk is found on the max level, and this is already a sub-interval,
                            //then index+1 is already known to be not found
                            if (chunk != null &&
                                level == maxSearchLevel &&
                                maxSearchLevel < DefaultSearchLevels)
                            {
                                feedChunkResult = new SwarmFeedChunk(
                                    index,
                                    chunk.Data.ToArray(),
                                    chunk.Hash);
                                return;
                            }
                            
                            //if current interval is completed
                            if (bestFoundResult.Level + 1 == lowestNotFoundLevel)
                            {
                                //if already had max resolution (lowest level)
                                if (bestFoundResult.Level == 0)
                                {
                                    feedChunkResult = new SwarmFeedChunk(
                                        bestFoundResult.Index,
                                        bestFoundResult.Chunk!.Data.ToArray(),
                                        bestFoundResult.Chunk.Hash);
                                    return;
                                }

                                //else, go more in deep with better interval
                                feedChunkResult = await RunLookupsAsync(
                                    chunkStore: chunkStore,
                                    baseIndex: bestFoundResult.Index,
                                    minSearchLevel: 0,
                                    maxSearchLevel: bestFoundResult.Level,
                                    bestFoundResult: bestFoundResult,
                                    lowestNotFoundLevel: lowestNotFoundLevel).ConfigureAwait(false);
                                return;
                            }
                            
                            //if results are inconsistent, retry from best level ahead
                            if (bestFoundResult.Level >= lowestNotFoundLevel)
                            {
                                feedChunkResult = await RunLookupsAsync(
                                    chunkStore: chunkStore,
                                    baseIndex: bestFoundResult.Index,
                                    minSearchLevel: bestFoundResult.Level,
                                    maxSearchLevel: maxSearchLevel,
                                    bestFoundResult: bestFoundResult,
                                    lowestNotFoundLevel: maxSearchLevel).ConfigureAwait(false);
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
