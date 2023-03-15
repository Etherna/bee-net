using Etherna.BeeNet.Clients.GatewayApi;
using Etherna.BeeNet.Feeds.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
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
        public Task<FeedChunk?> TryFindEpochFeedAsync(
            string account,
            byte[] topic,
            DateTimeOffset at,
            EpochFeedIndex? knownNearEpochIndex)
        {
            if (at < DateTimeOffset.FromUnixTimeSeconds((long)EpochFeedIndex.MinUnixTimeStamp) ||
                at > DateTimeOffset.FromUnixTimeSeconds((long)EpochFeedIndex.MaxUnixTimeStamp))
                throw new ArgumentOutOfRangeException(nameof(at), "Date is out of allowed range");

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
             * Phase 2) Find a starting point prior to the searched date. (bottom->up)
             * Verify if selected chunk on phase 1 exists, and if its timestamp is prior to the searched date.
             * If it doesn't exist, or if its time stamp is subsequent to searched date
             *   if epoch is right, try to search on left and adjust date as "chunk.Start - 1"
             *   else if epoch is left, try to search on parent.
             * Stops when a chunk with previous date is found, or when it reach max level limit on left chunk.
             * -> Output an existing chunk with previous date, or null if not found. If null, or chunk is found as a parent, skip phase 3.
             * 
             * Phase 3) Find the existing chunk with timestamp nearest and prior to the searched date. (top->down)
             * It starts from the output chunk of phase 2, and tries to get near as possibile to the final chunk.
             * It tries to get child epoch at date. If chunk exists and is prior, make recursion on it.
             * If it doesn't exist or it has timestamp subsequent to date.
             *   If child is right, try to get left and adjust date as "rightChild.Start - 1". Check again end eventually make recursion on it.
             *   If child is left return current chunk.
             * It stops when a valid chunk to continue recursion is not found, or when current chunk hit level 0 (max resolution).
             * -> Output the chunk with nearest timestamp prior to searched date.
             */

            var atUnixTime = (ulong)at.ToUnixTimeSeconds();

            // Find starting epoch index (bottom->up)
            var startEpoch = knownNearEpochIndex;
            if (startEpoch is not null)
            {
                //traverse parents until find a common ancestor
                while (startEpoch.Level != EpochFeedIndex.MaxLevel &&
                    !startEpoch.ContainsTime(atUnixTime))
                    startEpoch = startEpoch.GetParent();

                //if max level is reached and start epoch still doesn't contain the time, drop it
                if (!startEpoch.ContainsTime(atUnixTime))
                    startEpoch = null;
            }

            //if start epoch is null (known near was null or it was under the brother at max epoch level)
            startEpoch ??= new EpochFeedIndex(0, EpochFeedIndex.MaxLevel);
            if (!startEpoch.ContainsTime(atUnixTime))
                startEpoch = startEpoch.Right;

            // Find searched epoch index (top->down)
            return TryFindEpochFeedHelperAsync(
                account.HexToByteArray(),
                topic,
                atUnixTime,
                startEpoch,
                null);
        }

        public Task<string> UpdateEpochFeed(DateTime at, string payload)
        {
            throw new NotImplementedException();
        }

        // Helpers.
        /// <summary>
        /// Recursive finder function to find the version update chunk at time `at`
        /// </summary>
        /// <param name="at"></param>
        /// <param name="currentEpoch"></param>
        /// <param name="prevFoundChunk"></param>
        /// <returns></returns>
        private async Task<FeedChunk?> TryFindEpochFeedHelperAsync(
            byte[] account, byte[] topic, ulong at, EpochFeedIndex currentEpoch, FeedChunk? prevFoundChunk)
        {
            var currentChunk = await gatewayClient.GetChunkAsync(BuildReferenceHash(account, topic, currentEpoch));

            if (currentChunk == null)
            {
                // epoch not found on branch
                if (currentEpoch.IsLeft) // no lower resolution
                    return prevFoundChunk;

                // traverse earlier branch
                return await TryFindEpochFeedHelperAsync(currentEpoch.Start - 1, currentEpoch.Left, prevFoundChunk);
            }

            // epoch found
            // check if timestamp is later then target
            var ts = feeds.UpdatedAt(currentChunk);

            if (ts > at)
            {
                if (currentEpoch.IsLeft)
                    return prevFoundChunk;

                return await TryFindEpochFeedHelperAsync(currentEpoch.Start - 1, currentEpoch.Left, prevFoundChunk);
            }

            if (currentEpoch.Level == 0)
                return currentChunk;

            return await TryFindEpochFeedAtHelperAsync(at, currentEpoch.GetChildAt(at), currentChunk);
        }
    }
}