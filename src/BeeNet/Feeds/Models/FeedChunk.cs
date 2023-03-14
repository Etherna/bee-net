using Epoche;
using Etherna.BeeNet.Extensions;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Etherna.BeeNet.Feeds.Models
{
    public class FeedChunk
    {
        // Consts.
        public const int AccountBytesLength = 20;
        public const int IdentifierBytesLength = 32;
        public const int IndexBytesLength = 32;
        public const int MaxPayloadBytesSize = 4096; //4kB
        public const int MaxContentPayloadBytesSize = MaxPayloadBytesSize
                                                    - TimeStampByteSize; //creation timestamp
        public const int TimeStampByteSize = sizeof(ulong);
        public const int TopicBytesLength = 32;

        // Constructor.
        public FeedChunk(
            FeedIndexBase index,
            byte[] payload,
            string referenceHash)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < TimeStampByteSize)
                throw new ArgumentOutOfRangeException(nameof(payload),
                    $"Payload can't be shorter than {TimeStampByteSize} bytes");
            if (payload.Length > MaxPayloadBytesSize)
                throw new ArgumentOutOfRangeException(nameof(payload),
                    $"Payload can't be longer than {MaxPayloadBytesSize} bytes");

            Index = index ?? throw new ArgumentNullException(nameof(index));
            Payload = Array.AsReadOnly(payload);
            ReferenceHash = referenceHash ?? throw new ArgumentNullException(nameof(referenceHash));
        }

        // Properties.
        public FeedIndexBase Index { get; }
        public ReadOnlyCollection<byte> Payload { get; }
        public string ReferenceHash { get; }

        // Methods.
        public byte[] GetContentPayload() =>
            Payload.Skip(TimeStampByteSize).ToArray();

        public DateTimeOffset GetTimeStamp()
        {
            var unixTimeStamp = Payload.Take(TimeStampByteSize).ToArray().ByteArrayToUnixDateTime();
            return DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);
        }

        // Static helpers.
        public static byte[] BuildChunkPayload(byte[] contentPayload)
        {
            if (contentPayload.Length > MaxContentPayloadBytesSize)
                throw new ArgumentOutOfRangeException(nameof(contentPayload),
                    $"Content payload can't be longer than {MaxContentPayloadBytesSize} bytes");

            var chunkPayload = new byte[TimeStampByteSize + contentPayload.Length];
            var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            unixNow.UnixDateTimeToByteArray().CopyTo(chunkPayload, 0);
            contentPayload.CopyTo(chunkPayload, TimeStampByteSize);

            return chunkPayload;
        }

        public static byte[] BuildIdentifier(byte[] topic, FeedIndexBase index)
        {
            if (topic.Length != TopicBytesLength)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");

            var newArray = new byte[TopicBytesLength + IndexBytesLength];
            topic.CopyTo(newArray, 0);
            index.MarshalBinary.CopyTo(newArray, topic.Length);

            return Keccak256.ComputeHash(newArray);
        }

        public static string BuildReferenceHash(string account, byte[] topic, FeedIndexBase index) =>
            BuildReferenceHash(account, BuildIdentifier(topic, index));

        public static string BuildReferenceHash(byte[] account, byte[] topic, FeedIndexBase index) =>
            BuildReferenceHash(account, BuildIdentifier(topic, index));

        public static string BuildReferenceHash(string account, byte[] identifier)
        {
            if (!account.IsValidEthereumAddressHexFormat())
                throw new ArgumentException("Value is not a valid ethereum account", nameof(account));

            return BuildReferenceHash(account.HexToByteArray(), identifier);
        }

        public static string BuildReferenceHash(byte[] account, byte[] identifier)
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
    }
}
