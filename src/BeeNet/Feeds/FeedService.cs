using Epoche;
using Etherna.BeeNet.Clients.GatewayApi;
using Etherna.BeeNet.Feeds.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Feeds
{
    public class FeedService : IFeedService
    {
        // Consts.
        public const int AccountBytesLength = 20;
        public const int IdentifierBytesLength = 32;
        public const int IndexBytesLength = 32;
        public const int TopicBytesLength = 32;

        // Fields.
        private readonly IBeeGatewayClient gatewayClient;

        // Constructor.
        public FeedService(IBeeGatewayClient gatewayClient)
        {
            this.gatewayClient = gatewayClient;
        }

        // Methods.
        public byte[] GetIdentifier(byte[] topic, FeedIndexBase index)
        {
            if (topic.Length != TopicBytesLength)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");

            var newArray = new byte[TopicBytesLength + IndexBytesLength];
            topic.CopyTo(newArray, 0);
            index.MarshalBinary.CopyTo(newArray, topic.Length);

            return Keccak256.ComputeHash(newArray);
        }

        public string GetReferenceHash(string account, byte[] topic, FeedIndexBase index) =>
            GetReferenceHash(account, GetIdentifier(topic, index));

        public string GetReferenceHash(byte[] account, byte[] topic, FeedIndexBase index) =>
            GetReferenceHash(account, GetIdentifier(topic, index));

        public string GetReferenceHash(string account, byte[] identifier)
        {
            if (!account.IsValidEthereumAddressHexFormat())
                throw new ArgumentException("Value is not a valid ethereum account", nameof(account));

            return GetReferenceHash(account.HexToByteArray(), identifier);
        }

        public string GetReferenceHash(byte[] account, byte[] identifier)
        {
            if (account.Length != AccountBytesLength)
                throw new ArgumentOutOfRangeException(nameof(account), "Invalid account length");
            if (identifier.Length != IdentifierBytesLength)
                throw new ArgumentOutOfRangeException(nameof(identifier), "Invalid identifier length");

            var newArray = new byte[IdentifierBytesLength + AccountBytesLength];
            identifier.CopyTo(newArray, 0);
            account.CopyTo(newArray, IdentifierBytesLength);

            return Keccak256.ComputeHash(newArray).ToHex();
        }

        public Task<FeedChunk?> TryFindEpochFeedAsync(string account, byte[] topic, DateTimeOffset at)
        {
            var atUnixTime = (ulong)at.ToUnixTimeSeconds();
            var startEpoch = new EpochFeedIndex(0, EpochFeedIndex.MaxLevel);

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
        /// <param name="epoch"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        private async Task<FeedChunk?> TryFindEpochFeedHelperAsync(
            byte[] account, byte[] topic, ulong at, EpochFeedIndex epoch, FeedChunk ch)
        {
            var uch = await gatewayClient.GetChunkAsync(GetReferenceHash(account, topic, epoch));

            if (uch == null)
            {
                // epoch not found on branch
                if (epoch.IsLeft) // no lower resolution
                    return ch;

                // traverse earlier branch
                return await TryFindEpochFeedHelperAsync(epoch.Start - 1, epoch.Left, ch);
            }

            // epoch found
            // check if timestamp is later then target
            var ts = feeds.UpdatedAt(uch);

            if (ts > at)
            {
                if (epoch.IsLeft)
                    return ch;

                return await TryFindEpochFeedHelperAsync(epoch.Start - 1, epoch.Left, ch);
            }

            if (epoch.Level == 0)
                return uch;

            return await TryFindEpochFeedAtHelperAsync(at, epoch.GetChildAt(at), uch);
        }
    }
}