//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Epoche;
using Etherna.BeeNet.Extensions;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

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
        public const int MinPayloadByteSize = TimeStampByteSize;
        public const string ReferenceHashRegex = "^[A-Fa-f0-9]{64}$";
        public const int TimeStampByteSize = sizeof(ulong);
        public const int TopicBytesLength = 32;

        // Constructor.
        public FeedChunk(
            FeedIndexBase index,
            byte[] payload,
            string referenceHash)
        {
            ArgumentNullException.ThrowIfNull(payload, nameof(payload));
            ArgumentNullException.ThrowIfNull(referenceHash, nameof(referenceHash));

            if (payload.Length < MinPayloadByteSize)
                throw new ArgumentOutOfRangeException(nameof(payload),
                    $"Payload can't be shorter than {TimeStampByteSize} bytes");
            if (payload.Length > MaxPayloadBytesSize)
                throw new ArgumentOutOfRangeException(nameof(payload),
                    $"Payload can't be longer than {MaxPayloadBytesSize} bytes");

            if (!Regex.IsMatch(referenceHash, ReferenceHashRegex))
                throw new ArgumentException("Not a valid swarm hash", nameof(referenceHash));

            Index = index ?? throw new ArgumentNullException(nameof(index));
            Payload = Array.AsReadOnly(payload);
            ReferenceHash = referenceHash;
        }

        // Properties.
        public FeedIndexBase Index { get; }
        public ReadOnlyCollection<byte> Payload { get; }
        public string ReferenceHash { get; }

        // Methods.
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not FeedChunk objFeedChunk) return false;
            return GetType() == obj.GetType() &&
                Index.Equals(objFeedChunk.Index) &&
                Payload.SequenceEqual(objFeedChunk.Payload) &&
                ReferenceHash.Equals(objFeedChunk.ReferenceHash, StringComparison.Ordinal);
        }

        public byte[] GetContentPayload() =>
            Payload.Skip(TimeStampByteSize).ToArray();

        public override int GetHashCode() =>
            Index.GetHashCode() ^
            Payload.GetHashCode() ^
            ReferenceHash.GetHashCode(StringComparison.InvariantCulture);

        public DateTimeOffset GetTimeStamp()
        {
            var unixTimeStamp = Payload.Take(TimeStampByteSize).ToArray().ByteArrayToUnixDateTime();
            return DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);
        }

        // Static helpers.
        public static byte[] BuildChunkPayload(byte[] contentPayload, ulong? timestamp = null)
        {
            ArgumentNullException.ThrowIfNull(contentPayload, nameof(contentPayload));

            if (contentPayload.Length > MaxContentPayloadBytesSize)
                throw new ArgumentOutOfRangeException(nameof(contentPayload),
                    $"Content payload can't be longer than {MaxContentPayloadBytesSize} bytes");

            var chunkPayload = new byte[TimeStampByteSize + contentPayload.Length];
            timestamp ??= (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            timestamp.Value.UnixDateTimeToByteArray().CopyTo(chunkPayload, 0);
            contentPayload.CopyTo(chunkPayload, TimeStampByteSize);

            return chunkPayload;
        }

        public static byte[] BuildIdentifier(byte[] topic, FeedIndexBase index)
        {
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));
            ArgumentNullException.ThrowIfNull(index, nameof(index));

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
            ArgumentNullException.ThrowIfNull(account, nameof(account));
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));

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
