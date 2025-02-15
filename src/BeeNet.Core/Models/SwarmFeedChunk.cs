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

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
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
        public const int IdentifierSize = 32;
        public const int IndexSize = 32;
        public const int MaxChunkSize = MinChunkSize + DataSize;
        public const int MaxPayloadSize = DataSize - TimeStampSize; //creation timestamp
        public const int MinChunkSize = SwarmHash.HashSize + SwarmSignature.SignatureSize + SpanSize;
        public const int MinDataSize = TimeStampSize;
        public const int TimeStampSize = sizeof(ulong);
        public const int TopicSize = 32;

        // Constructor.
        public SwarmFeedChunk(
            FeedIndexBase index,
            byte[] data,
            SwarmHash hash) :
            base(hash, data)
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
                Hash.Equals(objFeedChunk.Hash) &&
                Data.Span.SequenceEqual(objFeedChunk.Data.Span) &&
                Index.Equals(objFeedChunk.Index) &&
                Span.Span.SequenceEqual(objFeedChunk.Span.Span);
        }
        
        public override int GetHashCode() =>
            Hash.GetHashCode() ^
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

        public static SwarmHash BuildHash(EthAddress owner, byte[] topic, FeedIndexBase index, IHasher hasher) =>
            BuildHash(owner, BuildIdentifier(topic, index, hasher), hasher);

        public static SwarmHash BuildHash(EthAddress owner, byte[] identifier, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));

            if (identifier.Length != IdentifierSize)
                throw new ArgumentOutOfRangeException(nameof(identifier), "Invalid identifier length");
            
            return hasher.ComputeHash(identifier.Concat(owner.ToByteArray()).ToArray());
        }

        public static byte[] BuildIdentifier(byte[] topic, FeedIndexBase index, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(index, nameof(index));
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));

            if (topic.Length != TopicSize)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");

            var newArray = new byte[TopicSize + IndexSize];
            topic.CopyTo(newArray, 0);
            index.MarshalBinary().CopyTo(newArray.AsMemory()[topic.Length..]);

            return hasher.ComputeHash(newArray);
        }
    }
}
