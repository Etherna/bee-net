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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmSequenceFeed(
        EthAddress owner,
        SwarmFeedTopic topic,
        TimeProvider? customTimeProvider = null)
        : SwarmFeedBase(owner, topic)
    {
        // Consts.
        private const int DefaultSearchLevels = 8;

        // Fields.
        /*
         * We can initialize new hashers here because hashers inside the pool are not reusable,
         * and we are not interested into injecting mocked hashers with tests.
         */
        private readonly ResourcePool<Hasher> hasherPool = new(() => new Hasher());
        private readonly TimeProvider timeProvider = customTimeProvider ?? TimeProvider.System;

        // Properties.
        public override SwarmFeedType Type => SwarmFeedType.Sequence;

        // Methods.
        public override async Task<SwarmFeedChunkBase> BuildNextFeedChunkAsync(
            ReadOnlyMemory<byte> data,
            SwarmFeedIndexBase? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            SwarmChunkBmt chunkBmt,
            DateTimeOffset? timestamp = null)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            
            return await BuildNextFeedChunkAsync(
                data,
                knownNearIndex as SwarmSequenceFeedIndex,
                chunkStore,
                chunkBmt).ConfigureAwait(false);
        }

        public async Task<SwarmSequenceFeedChunk> BuildNextFeedChunkAsync(
            ReadOnlyMemory<byte> data,
            SwarmSequenceFeedIndex? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            SwarmChunkBmt chunkBmt)
        {
            ArgumentNullException.ThrowIfNull(chunkBmt);
            
            // Find last published chunk.
            var lastFeedChunk = await TryFindLastFeedChunkAsync(knownNearIndex, chunkStore).ConfigureAwait(false);

            // Define next sequence index.
            var nextSequenceIndex = lastFeedChunk is null ?
                new SwarmSequenceFeedIndex(0) :
                (SwarmSequenceFeedIndex)lastFeedChunk.Index.GetNext(0);

            // Create new chunk.
            return new SwarmSequenceFeedChunk(
                Topic,
                nextSequenceIndex,
                BuildIdentifier(nextSequenceIndex, chunkBmt.Hasher),
                Owner,
                SwarmSequenceFeedChunk.BuildInnerChunk(data, chunkBmt),
                null);
        }

        public override async Task<SwarmFeedChunkBase?> TryFindFeedChunkAtAsync(
            long at,
            SwarmFeedIndexBase? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            Hasher hasher)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            
            return await TryFindLastFeedChunkAsync(
                knownNearIndex as SwarmSequenceFeedIndex,
                chunkStore).ConfigureAwait(false);
        }

        public async Task<SwarmSequenceFeedChunk?> TryFindLastFeedChunkAsync(
            SwarmSequenceFeedIndex? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            TimeSpan? requestsCustomTimeout = null)
        {
            // First lookup at the knownNearIndex, or default index(0).
            var hasher = hasherPool.GetResource();
            
            var chunk = await TryGetFeedChunkAsync(
                knownNearIndex ?? new SwarmSequenceFeedIndex(0),
                chunkStore,
                hasher).ConfigureAwait(false);
                
            hasherPool.ReturnResource(hasher);

            if (chunk is not SwarmSequenceFeedChunk sequenceFeedChunk)
                return null;

            // If chunk exists, start a recursive concurrent lookup.
            return await RunLookupsAsync(
                chunkStore,
                DefaultSearchLevels,
                sequenceFeedChunk,
                requestsCustomTimeout).ConfigureAwait(false);
        }

        // Helpers.
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
        private async Task<SwarmSequenceFeedChunk?> RunLookupsAsync(
            IReadOnlyChunkStore chunkStore,
            int maxSearchLevel,
            SwarmSequenceFeedChunk bestFoundChunk,
            TimeSpan? requestsCustomTimeout = null)
        {
            // Lookups on all the levels are requested concurrently. As soon as the highest existing chunk
            // of the searched interval is identified, the result is determined: this happens when a chunk
            // is found at a level, and all the greater levels have already returned as not found. At that
            // point any other still pending lookup is unnecessary, so it gets cancelled, and we can reply
            // without waiting for it.
            using var pendingLookupsCts = new CancellationTokenSource();
            using var semaphore = new SemaphoreSlim(1, 1);
            List<Task> tasks = [];

            SwarmSequenceFeedIndex baseIndex = (SwarmSequenceFeedIndex)bestFoundChunk.Index;
            int bestFoundLevel = 0;
            List<int> notFoundLevels = [];
            SwarmSequenceFeedChunk? feedChunkResult = null;

            for (var l = 1; l <= maxSearchLevel; l++)
            {
                var level = l;
                tasks.Add(Task.Run(async () =>
                {
                    // Init hasher.
                    var hasher = hasherPool.GetResource();
                    try
                    {
                        // Exec lookup. Each request has its own timeout, and is also cancelled as soon
                        // as the result is determined by another concurrent lookup.
                        using var timeoutCts = new CancellationTokenSource(requestsCustomTimeout ?? DefaultTimeout, timeProvider);
                        using var requestCts = CancellationTokenSource.CreateLinkedTokenSource(pendingLookupsCts.Token, timeoutCts.Token);

                        var index = new SwarmSequenceFeedIndex(baseIndex.Value + ((ulong)1 << level) - 1);
                        var chunk = await TryGetFeedChunkAsync(
                            index,
                            chunkStore,
                            hasher,
                            requestCts.Token).ConfigureAwait(false)
                            as SwarmSequenceFeedChunk;

                        // Evaluate result. Use semaphore on evaluation because of concurrent requests.
                        await semaphore.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            // Skip if the result is already determined, or a better chunk has already been found.
                            if (feedChunkResult is not null || level < bestFoundLevel)
                                return;

                            // Update edge results.
                            if (chunk is null)
                            {
                                notFoundLevels.Add(level);
                            }
                            else
                            {
                                bestFoundChunk = chunk;
                                bestFoundLevel = level;

                                // If any lower level failed to lookup an existing chunk (false negative),
                                // remove its wrong "not found" result.
                                notFoundLevels.RemoveAll(nfl => nfl < bestFoundLevel);
                            }

                            // The best found chunk is the highest existing one only when all the levels
                            // greater than it have returned as not found. Until then, a greater level
                            // could still return a chunk and move the edge forward.
                            if (notFoundLevels.Count != maxSearchLevel - bestFoundLevel)
                                return;

                            // The highest existing chunk of the interval has been found: terminate the other
                            // still pending lookups, because they are unnecessary.
                            await pendingLookupsCts.CancelAsync().ConfigureAwait(false);

                            // Reply with the best found chunk when the interval can't be narrowed further:
                            // no chunk has been found after the base, or a chunk has been found on the upper
                            // edge of a sub-interval (where the next index is already known as not existing).
                            // Otherwise, narrow the search inside the found interval.
                            if (bestFoundLevel == 0 ||
                                (bestFoundLevel == maxSearchLevel && maxSearchLevel < DefaultSearchLevels))
                                feedChunkResult = bestFoundChunk;
                            else
                                feedChunkResult = await RunLookupsAsync(
                                    chunkStore,
                                    bestFoundLevel,
                                    bestFoundChunk,
                                    requestsCustomTimeout).ConfigureAwait(false);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                    catch (OperationCanceledException) { }
                    finally
                    {
                        hasherPool.ReturnResource(hasher);
                    }
                }));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return feedChunkResult;
        }
        
        public override SwarmFeedChunkBase SocToFeedChunk(SwarmSoc soc, SwarmFeedIndexBase index) =>
            SwarmSequenceFeedChunk.BuildFromSoc(soc, index, Topic);
    }
}
