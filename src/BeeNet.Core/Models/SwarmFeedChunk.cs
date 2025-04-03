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
using Etherna.BeeNet.Stores;
using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class SwarmFeedChunk : SwarmChunk
    {
        // Consts.
        public const int IdentifierSize = 32;
        public const int MaxChunkSize = MinChunkSize + DataSize;
        public const int MaxPayloadSize = DataSize - TimeStampSize; //creation timestamp
        public const int MinChunkSize = SwarmHash.HashSize + SwarmSignature.SignatureSize + SpanSize;
        public const int MinDataSize = TimeStampSize;
        public const int TimeStampSize = sizeof(ulong);
        public const int TopicSize = 32;

        // Constructor.
        public SwarmFeedChunk(
            SwarmFeedIndexBase index,
            ReadOnlyMemory<byte> feedChunkPayload,
            SwarmHash hash) :
            base(hash, feedChunkPayload)
        {
            if (feedChunkPayload.Length < MinDataSize)
                throw new ArgumentOutOfRangeException(
                    nameof(feedChunkPayload),
                    $"Feed chunk payload can't be shorter than {TimeStampSize} bytes");
            
            Index = index ?? throw new ArgumentNullException(nameof(index));
        }
        
        // Static builders.
        public static SwarmFeedChunk BuildFromFeed(
            SwarmFeedBase feed,
            SwarmFeedIndexBase index,
            ReadOnlyMemory<byte> contentData,
            IHasher hasher,
            DateTimeOffset? timestamp = null)
        {
            ArgumentNullException.ThrowIfNull(feed, nameof(feed));
            
            var chunkPayload = BuildFeedChunkPayload(contentData, timestamp);
            var chunkHash = BuildHash(feed.Owner, feed.Topic, index, hasher);

            return new SwarmFeedChunk(index, chunkPayload, chunkHash);
        }

        // Properties.
        public SwarmFeedIndexBase Index { get; }
        public ReadOnlyMemory<byte> Payload => Data[TimeStampSize..];
        public DateTimeOffset TimeStamp => Data[..TimeStampSize].Span.UnixTimeSecondsToDateTimeOffset();

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
            Data.GetHashCode() ^
            Index.GetHashCode() ^
            Span.GetHashCode();
        
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
        public async Task<(SwarmChunk, SingleOwnerChunk)> UnwrapChunkAndSocAsync(
            bool resolveLegacyPayload,
            IHasher hasher,
            IChunkStore? chunkStore = null)
        {
            if (resolveLegacyPayload && chunkStore == null)
                throw new ArgumentNullException(nameof(chunkStore));
            
            var (soc, chunkHash) = SingleOwnerChunk.BuildFromBytes(Data, hasher);
            
            // Check if is legacy payload. Possible lengths:
            if (resolveLegacyPayload &&
                soc.ChunkData.Length is
                16 + SwarmHash.HashSize or   // unencrypted ref: span+timestamp+ref => 8+8+32=48
                16 + SwarmHash.HashSize * 2) // encrypted ref: span+timestamp+ref+decryptKey => 8+8+64=80
            {
                var hash = new SwarmHash(soc.ChunkData[16..]);
                return (await chunkStore!.GetAsync(hash).ConfigureAwait(false), soc);
            }

            return (new SwarmChunk(
                chunkHash,
                soc.ChunkData), soc);
        }
        
        // Static methods.
        public static byte[] BuildFeedChunkPayload(ReadOnlyMemory<byte> payload, DateTimeOffset? timestamp = null)
        {
            if (payload.Length > MaxPayloadSize)
                throw new ArgumentOutOfRangeException(nameof(payload),
                    $"Payload can't be longer than {MaxPayloadSize} bytes");

            var chunkData = new byte[TimeStampSize + payload.Length];
            
            var timestampByteArray = timestamp.HasValue ?
                timestamp.Value.ToUnixTimeSecondsByteArray() :
                DateTimeOffset.UtcNow.ToUnixTimeSecondsByteArray();
            timestampByteArray.CopyTo(chunkData, 0);
            
            payload.CopyTo(chunkData.AsMemory()[TimeStampSize..]);

            return chunkData;
        }
        
        public static SwarmHash BuildHash(
            EthAddress owner,
            ReadOnlyMemory<byte> topic,
            SwarmFeedIndexBase index,
            IHasher hasher) =>
            BuildHash(owner, BuildIdentifier(topic, index, hasher), hasher);

        public static SwarmHash BuildHash(EthAddress owner, ReadOnlyMemory<byte> identifier, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));

            if (identifier.Length != IdentifierSize)
                throw new ArgumentOutOfRangeException(nameof(identifier), "Invalid identifier length");
            
            return hasher.ComputeHash([identifier, owner.ToReadOnlyMemory()]);
        }
        
        public static byte[] BuildIdentifier(ReadOnlyMemory<byte> topic, SwarmFeedIndexBase index, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(index, nameof(index));

            if (topic.Length != TopicSize)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");

            return hasher.ComputeHash([topic, index.MarshalBinary()]);
        }
    }
}
