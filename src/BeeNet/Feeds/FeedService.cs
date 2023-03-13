using Epoche;
using Etherna.BeeNet.Clients.GatewayApi;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;
using System.IO;
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
        public byte[] GetIdentifier(byte[] topic, IFeedIndex index)
        {
            if (topic.Length != TopicBytesLength)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");

            var newArray = new byte[TopicBytesLength + IndexBytesLength];
            topic.CopyTo(newArray, 0);
            index.MarshalBinary.CopyTo(newArray, topic.Length);

            return Keccak256.ComputeHash(newArray);
        }

        public string GetReferenceHash(string account, byte[] topic, IFeedIndex index) =>
            GetReferenceHash(account, GetIdentifier(topic, index));

        public string GetReferenceHash(byte[] account, byte[] topic, IFeedIndex index) =>
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

        public Task<(string, Stream)?> TryFindEpochFeedAsync(string account, byte[] topic, DateTime at)
        {
            var atUnixTime = (ulong)new DateTimeOffset(at).ToUnixTimeSeconds();
        }

        public Task<string> UpdateEpochFeed(DateTime at, string payload)
        {
            throw new NotImplementedException();
        }
    }
}