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
using Etherna.BeeNet.Feeds;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public class SwarmFeedChunk : SwarmChunk
    {
        // Consts.
        public const int AccountSize = 20;
        public const int IdentifierSize = 32;
        public const int IndexSize = 32;
        public const int MaxPayloadSize = DataSize - TimeStampSize; //creation timestamp
        public const int MinDataSize = TimeStampSize;
        public const int TimeStampSize = sizeof(ulong);
        public const int TopicSize = 32;

        // Constructor.
        public SwarmFeedChunk(
            FeedIndexBase index,
            byte[] data,
            SwarmAddress address) :
            base(address, data)
        {
            if (data.Length < MinDataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be shorter than {TimeStampSize} bytes");
            
            Index = index ?? throw new ArgumentNullException(nameof(index));
        }

        // Properties.
        public FeedIndexBase Index { get; }
        public ReadOnlyMemory<byte> Payload => Data[TimeStampSize..];
        public DateTimeOffset TimeStamp
        {
            get
            {
                var unixTimeStamp = Data[..TimeStampSize].ToArray().ByteArrayToUnixDateTime();
                return DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);
            }
        }

        // Methods.
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not SwarmFeedChunk objFeedChunk) return false;
            return GetType() == obj.GetType() &&
                Address.Equals(objFeedChunk.Address) &&
                Data.Span.SequenceEqual(objFeedChunk.Data.Span) &&
                Index.Equals(objFeedChunk.Index) &&
                Span.Span.SequenceEqual(objFeedChunk.Span.Span);
        }
        
        public override int GetHashCode() =>
            Address.GetHashCode() ^
            _data.GetHashCode() ^
            Index.GetHashCode() ^
            _span.GetHashCode();

        // Static helpers.
        public static byte[] BuildChunkPayload(byte[] payload, ulong? timestamp = null)
        {
            ArgumentNullException.ThrowIfNull(payload, nameof(payload));

            if (payload.Length > MaxPayloadSize)
                throw new ArgumentOutOfRangeException(nameof(payload),
                    $"Payload can't be longer than {MaxPayloadSize} bytes");

            var chunkData = new byte[TimeStampSize + payload.Length];
            timestamp ??= (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            timestamp.Value.UnixDateTimeToByteArray().CopyTo(chunkData, 0);
            payload.CopyTo(chunkData, TimeStampSize);

            return chunkData;
        }

        public static byte[] BuildIdentifier(byte[] topic, FeedIndexBase index)
        {
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));
            ArgumentNullException.ThrowIfNull(index, nameof(index));

            if (topic.Length != TopicSize)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");

            var newArray = new byte[TopicSize + IndexSize];
            topic.CopyTo(newArray, 0);
            index.MarshalBinary.CopyTo(newArray, topic.Length);

            return Keccak256.ComputeHash(newArray);
        }

        public static SwarmAddress BuildReferenceHash(string account, byte[] topic, FeedIndexBase index) =>
            BuildReferenceHash(account, BuildIdentifier(topic, index));

        public static SwarmAddress BuildReferenceHash(byte[] account, byte[] topic, FeedIndexBase index) =>
            BuildReferenceHash(account, BuildIdentifier(topic, index));

        public static SwarmAddress BuildReferenceHash(string account, byte[] identifier)
        {
            if (!account.IsValidEthereumAddressHexFormat())
                throw new ArgumentException("Value is not a valid ethereum account", nameof(account));

            return BuildReferenceHash(account.HexToByteArray(), identifier);
        }

        public static SwarmAddress BuildReferenceHash(byte[] account, byte[] identifier)
        {
            ArgumentNullException.ThrowIfNull(account, nameof(account));
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));

            if (account.Length != AccountSize)
                throw new ArgumentOutOfRangeException(nameof(account), "Invalid account length");
            if (identifier.Length != IdentifierSize)
                throw new ArgumentOutOfRangeException(nameof(identifier), "Invalid identifier length");

            return Keccak256.ComputeHash(identifier.Concat(account).ToArray());
        }
    }
}
