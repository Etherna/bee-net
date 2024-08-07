﻿// Copyright 2021-present Etherna SA
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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Models.Feeds;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public class FeedService(IBeeClient gatewayClient)
        : IFeedService
    {
        // Methods.
        public Task<SwarmFeedChunk> CreateNextEpochFeedChunkAsync(
            string account,
            byte[] topic,
            byte[] contentPayload,
            EpochFeedIndex? knownNearEpochIndex) =>
            CreateNextEpochFeedChunkAsync(account.HexToByteArray(), topic, contentPayload, knownNearEpochIndex);

        public async Task<SwarmFeedChunk> CreateNextEpochFeedChunkAsync(
            byte[] account,
            byte[] topic,
            byte[] contentPayload,
            EpochFeedIndex? knownNearEpochIndex)
        {
            var at = DateTimeOffset.UtcNow;

            // Find last published chunk.
            var lastEpochFeedChunk = await TryFindEpochFeedAsync(account, topic, at, knownNearEpochIndex).ConfigureAwait(false);

            // Define next epoch index.
            EpochFeedIndex nextEpochIndex;
            if (lastEpochFeedChunk is null)
            {
                nextEpochIndex = new EpochFeedIndex(0, EpochFeedIndex.MaxLevel);
                if (!nextEpochIndex.ContainsTime(at))
                    nextEpochIndex = nextEpochIndex.Right;
            }
            else
                nextEpochIndex = (EpochFeedIndex)lastEpochFeedChunk.Index.GetNext(at);

            // Create new chunk.
            var chunkPayload = SwarmFeedChunk.BuildChunkPayload(contentPayload, (ulong)at.ToUnixTimeSeconds());
            var chunkHash = SwarmFeedChunk.BuildHash(account, topic, nextEpochIndex, new Hasher());

            return new SwarmFeedChunk(nextEpochIndex, chunkPayload, chunkHash);
        }

        public Task<SwarmFeedChunk?> TryFindEpochFeedAsync(
            string account,
            byte[] topic,
            DateTimeOffset at,
            EpochFeedIndex? knownNearEpochIndex) =>
            TryFindEpochFeedAsync(account.HexToByteArray(), topic, at, knownNearEpochIndex);

        public async Task<SwarmFeedChunk?> TryFindEpochFeedAsync(
            byte[] account,
            byte[] topic,
            DateTimeOffset at,
            EpochFeedIndex? knownNearEpochIndex)
        {
            if (at < DateTimeOffset.FromUnixTimeSeconds((long)EpochFeedIndex.MinUnixTimeStamp) ||
                at > DateTimeOffset.FromUnixTimeSeconds((long)EpochFeedIndex.MaxUnixTimeStamp))
                throw new ArgumentOutOfRangeException(nameof(at), "Date is out of allowed range");

            var atUnixTime = (ulong)at.ToUnixTimeSeconds();

            /*
             * This look up is composed by different phases:
             * 
             * Phase 1) Find a starting epoch index to look up containing the date. (bottom->up)
             * 
             * This doesn't access to swarm network, so it ignores chunks' timestamps.
             * It starts from an optional well known existing epoch index passed by user, and tries to find index of an existing starting chunk.
             * Passed epoch index should be as near as possibile to the final chunk to maximize optimization.
             * Process is tollerant to wrong initial passed index, even if in this case it is not optimized.
             * 
             * It tries to find common anchestor between date and existing previously known epoch, if passed.
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
             * Stops when a chunk with previous date is found, or when it reach max level limit on left chunk.
             * 
             * -> Input: starting epoch index from phase 1.
             * <- Output: an existing chunk with prior date, or null if a chunk is not found. If null, skip phase 3.
             * 
             * ------------
             * Phase 3) Find the existing chunk with timestamp nearest and prior to the searched date. (top->down)
             * 
             * It starts from the output chunk of phase 2, and tries to get near as possibile to searched date, without pass it.
             * Is possible that, if the passed chunk is a left chunk, epoch index of passed chunk could not contain the "at" date.
             * In this case adjust the date as (chunk.Index.Right.Start - 1).
             * 
             * It tries to get child epoch at date from existing chunk. If chunk exists and is prior, make recursion on it.
             * If it doesn't exist or it has timestamp subsequent to date.
             *   If child is right, try to get left. Check again end eventually make recursion on it.
             *   If child is left return current chunk.
             * It stops when a valid chunk to continue recursion is not found, or when current chunk hit level 0 (max resolution).
             * 
             * -> Input: starting chunk from phase 2.
             * <- Output: the chunk with nearest timestamp prior to searched date.
             */

            // Phase 1)
            var startEpoch = FindStartingEpochOffline(knownNearEpochIndex, atUnixTime);

            // Phase 2)
            var startChunk = await TryFindStartingEpochChunkOnlineAsync(
                account,
                topic,
                atUnixTime,
                startEpoch).ConfigureAwait(false);

            if (startChunk is null)
                return null;

            // Phase 3)
            return await FindLastEpochChunkBeforeDateAsync(
                account,
                topic,
                atUnixTime,
                startChunk).ConfigureAwait(false);
        }

        public Task<SwarmFeedChunk?> TryGetFeedChunkAsync(string account, byte[] topic, FeedIndexBase index) =>
            TryGetFeedChunkAsync(account.HexToByteArray(), topic, index);

        public Task<SwarmFeedChunk?> TryGetFeedChunkAsync(byte[] account, byte[] topic, FeedIndexBase index) =>
            TryGetFeedChunkAsync(SwarmFeedChunk.BuildHash(account, topic, index, new Hasher()), index);

        public async Task<SwarmFeedChunk?> TryGetFeedChunkAsync(SwarmHash hash, FeedIndexBase index)
        {
            try
            {
                using var chunkStream = await gatewayClient.GetChunkStreamAsync(hash).ConfigureAwait(false);
                using var chunkMemoryStream = new MemoryStream();
                await chunkStream.CopyToAsync(chunkMemoryStream).ConfigureAwait(false);
                return new SwarmFeedChunk(index, chunkMemoryStream.ToArray(), hash);
            }
            catch (BeeNetApiException)
            {
                return null;
            }
        }

        // Helpers.
        internal async Task<SwarmFeedChunk> FindLastEpochChunkBeforeDateAsync(byte[] account, byte[] topic, ulong at, SwarmFeedChunk currentChunk)
        {
            // If currentChunk is at max resolution, return it.
            var currentIndex = (EpochFeedIndex)currentChunk.Index;
            if (currentIndex.Level == EpochFeedIndex.MinLevel)
                return currentChunk;

            // Normalize "at" date. Possibile if we are trying a left epoch, but date is contained at right.
            if (!currentIndex.ContainsTime(at))
                at = currentIndex.Right.Start - 1;

            // Try chunk on child epoch at date.
            var childIndexAtDate = currentIndex.GetChildAt(at);
            var childChunkAtDate = await TryGetFeedChunkAsync(account, topic, childIndexAtDate).ConfigureAwait(false);
            if (childChunkAtDate != null && (ulong)childChunkAtDate.TimeStamp.ToUnixTimeSeconds() <= at)
                return await FindLastEpochChunkBeforeDateAsync(account, topic, at, childChunkAtDate).ConfigureAwait(false);

            // Try left brother if different.
            if (childIndexAtDate.IsRight)
            {
                var childLeftChunk = await TryGetFeedChunkAsync(account, topic, childIndexAtDate.Left).ConfigureAwait(false);
                if (childLeftChunk != null) //to check timestamp is superfluous in this case
                    return await FindLastEpochChunkBeforeDateAsync(account, topic, at, childLeftChunk).ConfigureAwait(false);
            }

            return currentChunk;
        }

        /// <summary>
        /// Implement phase 1 of epoch chunk look up.
        /// </summary>
        /// <param name="knownNearEpoch">An optional epoch index with known existing chunk</param>
        /// <param name="at">The searched date</param>
        /// <returns>A starting epoch index</returns>
        internal static EpochFeedIndex FindStartingEpochOffline(EpochFeedIndex? knownNearEpoch, ulong at)
        {
            var startEpoch = knownNearEpoch;
            if (startEpoch is not null)
            {
                //traverse parents until find a common ancestor
                while (startEpoch.Level != EpochFeedIndex.MaxLevel &&
                    !startEpoch.ContainsTime(at))
                    startEpoch = startEpoch.Parent;

                //if max level is reached and start epoch still doesn't contain the time, drop it
                if (!startEpoch.ContainsTime(at))
                    startEpoch = null;
            }

            //if start epoch is null (known near was null or max epoch level is hit)
            startEpoch ??= new EpochFeedIndex(0, EpochFeedIndex.MaxLevel);
            if (!startEpoch.ContainsTime(at))
                startEpoch = startEpoch.Right;
            return startEpoch;
        }

        /// <summary>
        /// Implement phase 2 of epoch chunk look up.
        /// </summary>
        /// <param name="account">The SOC owner account</param>
        /// <param name="topic">The SOC topic</param>
        /// <param name="at">The searched date</param>
        /// <param name="epochIndex">The epoch to analyze containing current date</param>
        /// <returns>A tuple with found chunk (if any) and updated "at" date</returns>
        internal async Task<SwarmFeedChunk?> TryFindStartingEpochChunkOnlineAsync(byte[] account, byte[] topic, ulong at, EpochFeedIndex epochIndex)
        {
            // Try to get chunk payload on network.
            var chunk = await TryGetFeedChunkAsync(account, topic, epochIndex).ConfigureAwait(false);

            // If chunk exists and date is prior.
            if (chunk != null && (ulong)chunk.TimeStamp.ToUnixTimeSeconds() <= at)
                return chunk;

            // Else, if chunk is not found, or if chunk timestamp is later than target date.
            if (epochIndex.IsRight)                               //try left
                return await TryFindStartingEpochChunkOnlineAsync(account, topic, at, epochIndex.Left).ConfigureAwait(false);
            else if (epochIndex.Level != EpochFeedIndex.MaxLevel) //try parent
                return await TryFindStartingEpochChunkOnlineAsync(account, topic, at, epochIndex.Parent).ConfigureAwait(false);

            return null;
        }
    }
}