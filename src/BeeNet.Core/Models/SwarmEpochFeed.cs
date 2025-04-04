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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class SwarmEpochFeed(EthAddress owner, ReadOnlyMemory<byte> topic)
        : SwarmFeedBase(owner, topic)
    {
        // Properties.
        public override SwarmFeedType Type => SwarmFeedType.Epoch;

        // Methods.
        public override Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            ReadOnlyMemory<byte> contentData,
            SwarmFeedIndexBase? knownNearIndex,
            Func<IHasher> hasherBuilder,
            DateTimeOffset? timestamp = null)
        {
            ArgumentNullException.ThrowIfNull(hasherBuilder, nameof(hasherBuilder));
            
            if (knownNearIndex is not (null or SwarmEpochFeedIndex))
                throw new ArgumentException("Feed index bust be null or epoch index", nameof(knownNearIndex));
            return BuildNextFeedChunkAsync(
                chunkStore,
                contentData,
                knownNearIndex as SwarmEpochFeedIndex,
                hasherBuilder(),
                timestamp);
        }

        public async Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            ReadOnlyMemory<byte> contentData,
            SwarmEpochFeedIndex? knownNearIndex,
            IHasher hasher,
            DateTimeOffset? timestamp = null)
        {
            var at = DateTimeOffset.UtcNow;

            // Find last published chunk.
            var lastEpochFeedChunk = await TryFindFeedAtAsync(chunkStore, at, knownNearIndex, hasher).ConfigureAwait(false);

            // Define next epoch index.
            SwarmEpochFeedIndex nextEpochIndex;
            if (lastEpochFeedChunk is null)
            {
                nextEpochIndex = new SwarmEpochFeedIndex(0, SwarmEpochFeedIndex.MaxLevel, hasher);
                if (!nextEpochIndex.ContainsTime(at))
                    nextEpochIndex = nextEpochIndex.Right;
            }
            else
                nextEpochIndex = (SwarmEpochFeedIndex)lastEpochFeedChunk.Index.GetNext(at);

            // Create new chunk.
            return SwarmFeedChunk.BuildFromFeed(this, nextEpochIndex, contentData, hasher, timestamp);
        }

        public override Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            long at,
            SwarmFeedIndexBase? knownNearIndex,
            Func<IHasher> hasherBuilder)
        {
            ArgumentNullException.ThrowIfNull(hasherBuilder, nameof(hasherBuilder));
            
            if (knownNearIndex is not (null or SwarmEpochFeedIndex))
                throw new ArgumentException("Feed index bust be null or epoch index", nameof(knownNearIndex));
            return TryFindFeedAtAsync(
                chunkStore,
                DateTimeOffset.FromUnixTimeSeconds(at),
                knownNearIndex as SwarmEpochFeedIndex,
                hasherBuilder());
        }

        public async Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            DateTimeOffset at,
            SwarmEpochFeedIndex? knownNearEpochIndex,
            IHasher hasher)
        {
            if (at < DateTimeOffset.FromUnixTimeSeconds((long)SwarmEpochFeedIndex.MinUnixTimeStamp) ||
                at > DateTimeOffset.FromUnixTimeSeconds((long)SwarmEpochFeedIndex.MaxUnixTimeStamp))
                throw new ArgumentOutOfRangeException(nameof(at), "Date is out of allowed range");

            var atUnixTime = (ulong)at.ToUnixTimeSeconds();

            /*
             * This look up is composed by different phases:
             * 
             * Phase 1) Find a starting epoch index to look up containing the date. (bottom->up)
             * 
             * This doesn't access to swarm network, so it ignores chunks' timestamps.
             * It starts from an optional well known existing epoch index passed by user, and tries to find index of an existing starting chunk.
             * Passed epoch index should be as near as possible to the final chunk to maximize optimization.
             * Process is tolerant to wrong initial passed index, even if in this case it is not optimized.
             * 
             * It tries to find common ancestor between date and existing previously known epoch, if passed.
             * If a previously known epoch is not passed, start at max level with epoch containing the date.
             * 
             * -> Input: optional known existing epoch index, near to searched date.
             * <- Output: starting epoch index that could not exist as a chunk, or chunk timestamp could be subsequent to searched date,
             *    but epoch contains the date.
             * 
             * ------------
             * Phase 2) Find a starting chunk prior to the searched date. (bottom->up)
             * 
             * Verify if selected chunk on phase 1 exists, and if its timestamp is prior to the searched date.
             * If it doesn't exist, or if time stamp is subsequent to searched date,
             *   if epoch index is right, try to search on left,
             *   else if epoch index is left, try to search on parent.
             * Stops when a chunk with previous date is found, or when it reaches max level limit on left chunk.
             * 
             * -> Input: starting epoch index from phase 1.
             * <- Output: an existing chunk with prior date, or null if a chunk is not found. If null, skip phase 3.
             * 
             * ------------
             * Phase 3) Find the existing chunk with timestamp nearest and prior to the searched date. (top->down)
             * 
             * It starts from the output chunk of phase 2, and tries to get near as possible to searched date, without pass it.
             * Is possible that, if the passed chunk is a left chunk, epoch index of passed chunk could not contain the "at" date.
             * In this case adjust the date as (chunk.Index.Right.Start - 1).
             * 
             * It tries to get child epoch at date from existing chunk. If chunk exists and is prior, make recursion on it.
             * If it doesn't exist, or it has timestamp subsequent to date.
             *   If child is right, try to get left. Check again end eventually make recursion on it.
             *   If child is left return current chunk.
             * It stops when a valid chunk to continue recursion is not found, or when current chunk hit level 0 (max resolution).
             * 
             * -> Input: starting chunk from phase 2.
             * <- Output: the chunk with nearest timestamp prior to searched date.
             */

            // Phase 1)
            var startEpoch = FindStartingEpochOffline(knownNearEpochIndex, atUnixTime, hasher);

            // Phase 2)
            var startChunk = await TryFindStartingEpochChunkOnlineAsync(
                chunkStore,
                atUnixTime,
                startEpoch,
                hasher).ConfigureAwait(false);

            if (startChunk is null)
                return null;

            // Phase 3)
            return await FindLastEpochChunkBeforeDateAsync(
                chunkStore,
                atUnixTime,
                startChunk,
                hasher).ConfigureAwait(false);
        }

        // Helpers.
        internal async Task<SwarmFeedChunk> FindLastEpochChunkBeforeDateAsync(
            IReadOnlyChunkStore chunkStore,
            ulong at,
            SwarmFeedChunk currentChunk,
            IHasher hasher)
        {
            // If currentChunk is at max resolution, return it.
            var currentIndex = (SwarmEpochFeedIndex)currentChunk.Index;
            if (currentIndex.Level == SwarmEpochFeedIndex.MinLevel)
                return currentChunk;

            // Normalize "at" date. Possibile if we are trying a left epoch, but date is contained at right.
            if (!currentIndex.ContainsTime(at))
                at = currentIndex.Right.Start - 1;

            // Try chunk on child epoch at date.
            var childIndexAtDate = currentIndex.GetChildAt(at);
            var childChunkAtDate = await TryGetFeedChunkAsync(chunkStore, childIndexAtDate, hasher).ConfigureAwait(false);
            if (childChunkAtDate != null && (ulong)childChunkAtDate.TimeStamp.ToUnixTimeSeconds() <= at)
                return await FindLastEpochChunkBeforeDateAsync(chunkStore, at, childChunkAtDate, hasher).ConfigureAwait(false);

            // Try left brother if different.
            if (childIndexAtDate.IsRight)
            {
                var childLeftChunk = await TryGetFeedChunkAsync(chunkStore, childIndexAtDate.Left, hasher).ConfigureAwait(false);
                if (childLeftChunk != null) //to check timestamp is superfluous in this case
                    return await FindLastEpochChunkBeforeDateAsync(chunkStore, at, childLeftChunk, hasher).ConfigureAwait(false);
            }

            return currentChunk;
        }

        /// <summary>
        /// Implement phase 1 of epoch chunk look up.
        /// </summary>
        /// <param name="knownNearEpoch">An optional epoch index with known existing chunk</param>
        /// <param name="at">The searched date</param>
        /// <param name="hasher"></param>
        /// <returns>A starting epoch index</returns>
        internal static SwarmEpochFeedIndex FindStartingEpochOffline(
            SwarmEpochFeedIndex? knownNearEpoch,
            ulong at,
            IHasher hasher)
        {
            var startEpoch = knownNearEpoch;
            if (startEpoch is not null)
            {
                //traverse parents until find a common ancestor
                while (startEpoch.Level != SwarmEpochFeedIndex.MaxLevel &&
                    !startEpoch.ContainsTime(at))
                    startEpoch = startEpoch.Parent;

                //if max level is reached and start epoch still doesn't contain the time, drop it
                if (!startEpoch.ContainsTime(at))
                    startEpoch = null;
            }

            //if start epoch is null (known near was null or max epoch level is hit)
            startEpoch ??= new SwarmEpochFeedIndex(0, SwarmEpochFeedIndex.MaxLevel, hasher);
            if (!startEpoch.ContainsTime(at))
                startEpoch = startEpoch.Right;
            return startEpoch;
        }

        /// <summary>
        /// Implement phase 2 of epoch chunk look up.
        /// </summary>
        /// <param name="chunkStore">The chunk store</param>
        /// <param name="at">The searched date</param>
        /// <param name="epochIndex">The epoch to analyze containing current date</param>
        /// <returns>A tuple with found chunk (if any) and updated "at" date</returns>
        internal async Task<SwarmFeedChunk?> TryFindStartingEpochChunkOnlineAsync(
            IReadOnlyChunkStore chunkStore,
            ulong at,
            SwarmEpochFeedIndex epochIndex,
            IHasher hasher)
        {
            // Try to get chunk payload on network.
            var chunk = await TryGetFeedChunkAsync(chunkStore, epochIndex, hasher).ConfigureAwait(false);

            // If chunk exists and date is prior.
            if (chunk != null && (ulong)chunk.TimeStamp.ToUnixTimeSeconds() <= at)
                return chunk;

            // Else, if chunk is not found, or if chunk timestamp is later than target date.
            if (epochIndex.IsRight)                               //try left
                return await TryFindStartingEpochChunkOnlineAsync(chunkStore, at, epochIndex.Left, hasher).ConfigureAwait(false);
            else if (epochIndex.Level != SwarmEpochFeedIndex.MaxLevel) //try parent
                return await TryFindStartingEpochChunkOnlineAsync(chunkStore, at, epochIndex.Parent, hasher).ConfigureAwait(false);

            return null;
        }
    }
}