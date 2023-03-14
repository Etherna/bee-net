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