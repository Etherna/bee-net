using Etherna.BeeNet.Clients.GatewayApi;
using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Feeds.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Feeds
{
    public class FeedService : IFeedService
    {
        // Consts.

        // Fields.
        private readonly IBeeGatewayClient gatewayClient;

        // Constructor.
        public FeedService(IBeeGatewayClient gatewayClient)
        {
            this.gatewayClient = gatewayClient;
        }

        // Methods.
        public async Task<FeedChunk?> TryFindEpochFeedAsync(
            string account,
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
             * Phase 1) Find a starting epoch to look up containing the date. (bottom->up)
             * This doesn't access to swarm network, so it ignore chunks' timestamps.
             * It starts from an optional well known existing epoch index passed by user,
             * and tries to find the index of an existing chunk to use as start.
             * Starting chunk should be as near as possibile to the final chunk, without need to access the net.
             * It tries to find common anchestor between date and existing previously known epoch, if passed.
             * If a previously known epoch is not passed, start at max level with epoch containing the date.
             * -> Output a starting epoch index that could not exist as a chunk, or chunk timestamp could be subsequent to searched date,
             *    but epoch contains the date.
             * 
             * Phase 2) Find a starting chunk prior to the searched date. (bottom->up)
             * Verify if selected chunk on phase 1 exists, and if its timestamp is prior to the searched date.
             * If it doesn't exist, or if its time stamp is subsequent to searched date,
             *   if epoch is right, try to search on left,
             *   else if epoch is left, try to search on parent.
             * Stops when a chunk with previous date is found, or when it reach max level limit on left chunk.
             * -> Output an existing chunk with previous date, or null if not found. If null skip phase 3.
             * 
             * Phase 3) Find the existing chunk with timestamp nearest and prior to the searched date. (top->down)
             * It starts from the output chunk of phase 2, and tries to get near as possibile to the final chunk.
             * It tries to get child epoch at date. If chunk exists and is prior, make recursion on it.
             * If it doesn't exist or it has timestamp subsequent to date.
             *   If child is right, try to get left. Check again end eventually make recursion on it.
             *   If child is left return current chunk.
             * It stops when a valid chunk to continue recursion is not found, or when current chunk hit level 0 (max resolution).
             * -> Output the chunk with nearest timestamp prior to searched date.
             */

            // Phase 1)
            var startEpoch = FindStartingEpochOffline(knownNearEpochIndex, atUnixTime);

            // Phase 2)
            var accountByteArray = account.HexToByteArray();
            var startChunk = await TryFindStartingChunkOnlineAsync(
                accountByteArray,
                topic,
                atUnixTime,
                startEpoch);

            if (startChunk is null)
                return null;

            // Phase 3)
            return await FindLastEpochChunkBeforeDateAsync(
                accountByteArray,
                topic,
                atUnixTime,
                startChunk);
        }

        public Task<string> UpdateEpochFeed(DateTime at, string payload)
        {
            throw new NotImplementedException();
        }

        // Helpers.
        private async Task<FeedChunk> FindLastEpochChunkBeforeDateAsync(byte[] account, byte[] topic, ulong at, FeedChunk currentChunk)
        {
            var currentIndex = (EpochFeedIndex)currentChunk.Index;
            var childIndexAtDate = currentIndex.GetChildAt(at);

            // If currentChunk is at max resolution, return it.
            if (currentIndex.Level == EpochFeedIndex.MinLevel)
                return currentChunk;

            // Try chunk on child epoch at date.
            var childChunkAtDate = await TryGetFeedChunkAsync(account, topic, childIndexAtDate);
            if (childChunkAtDate != null && (ulong)childChunkAtDate.GetTimeStamp().ToUnixTimeSeconds() <= at)
                return await FindLastEpochChunkBeforeDateAsync(account, topic, at, childChunkAtDate);

            // Try left brother if different.
            if (childIndexAtDate.IsRight)
            {
                var childLeftChunk = await TryGetFeedChunkAsync(account, topic, childIndexAtDate);
                if (childLeftChunk != null && (ulong)childLeftChunk.GetTimeStamp().ToUnixTimeSeconds() <= at)
                    return await FindLastEpochChunkBeforeDateAsync(account, topic, at, childLeftChunk);
            }

            return currentChunk;
        }

        private static EpochFeedIndex FindStartingEpochOffline(EpochFeedIndex? knownNearEpoch, ulong at)
        {
            var startEpoch = knownNearEpoch;
            if (startEpoch is not null)
            {
                //traverse parents until find a common ancestor
                while (startEpoch.Level != EpochFeedIndex.MaxLevel &&
                    !startEpoch.ContainsTime(at))
                    startEpoch = startEpoch.GetParent();

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
        /// Implement phase 2 of chunk look up.
        /// </summary>
        /// <param name="account">The SOC owner account</param>
        /// <param name="topic">The SOC topic</param>
        /// <param name="at">The searched date</param>
        /// <param name="epochIndex">The epoch to analyze containing current date</param>
        /// <returns>A tuple with found chunk (if any) and updated "at" date</returns>
        private async Task<FeedChunk?> TryFindStartingChunkOnlineAsync(byte[] account, byte[] topic, ulong at, EpochFeedIndex epochIndex)
        {
            // Try get chunk payload on network.
            var chunk = await TryGetFeedChunkAsync(account, topic, epochIndex);

            // If chunk exists and date is prior.
            if (chunk != null && (ulong)chunk.GetTimeStamp().ToUnixTimeSeconds() <= at)
                return chunk;

            // Else, if chunk is not found, or if chunk timestamp is later than target date.
            if (epochIndex.IsRight)                               //try left
                return await TryFindStartingChunkOnlineAsync(account, topic, at, epochIndex.Left);
            else if (epochIndex.Level != EpochFeedIndex.MaxLevel) //try parent
                return await TryFindStartingChunkOnlineAsync(account, topic, at, epochIndex.GetParent());

            return null;
        }

        private async Task<FeedChunk?> TryGetFeedChunkAsync(byte[] account, byte[] topic, FeedIndexBase index)
        {
            var chunkReference = FeedChunk.BuildReferenceHash(account, topic, index);
            try
            {
                using var chunkStream = await gatewayClient.GetChunkAsync(chunkReference);
                using var chunkMemoryStream = new MemoryStream();
                chunkStream.CopyTo(chunkMemoryStream);
                return new FeedChunk(index, chunkMemoryStream.ToArray(), chunkReference);
            }
            catch (BeeNetGatewayApiException)
            {
                return null;
            }
        }
    }
}