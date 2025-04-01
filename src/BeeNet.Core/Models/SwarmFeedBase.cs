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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields")]
    public abstract class SwarmFeedBase
    {
        // Consts.
        public const int IdentifierSize = 32;
        public const int TopicSize = 32;
        
        // Fields.
        protected readonly byte[] _topic;

        // Constructors.
        protected SwarmFeedBase(EthAddress owner, byte[] topic)
        {
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));

            if (topic.Length != TopicSize)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");
            
            Owner = owner;
            _topic = topic;
        }

        // Properties.
        public EthAddress Owner { get; }
        public ReadOnlyMemory<byte> Topic => _topic.AsMemory();
        public abstract SwarmFeedType Type { get; }
        
        // Methods.
        public SwarmHash BuildHash(SwarmFeedIndexBase index, IHasher hasher) =>
            BuildHash(Owner, BuildIdentifier(index, hasher), hasher);

        public byte[] BuildIdentifier(SwarmFeedIndexBase index, IHasher hasher) =>
            BuildIdentifier(_topic, index, hasher);
        
        public abstract Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            byte[] contentPayload,
            SwarmFeedIndexBase? knownNearIndex,
            Func<IHasher> hasherBuilder);

        /// <summary>
        /// Try to find feed at a given time
        /// </summary>
        /// <param name="chunkStore">The chunk store</param>
        /// <param name="at">The time to search</param>
        /// <param name="knownNearIndex">Another known existing index, near to looked time. Helps to perform lookup quicker</param>
        /// <param name="hasherBuilder"></param>
        /// <returns>The found feed chunk, or null</returns>
        public abstract Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            long at,
            SwarmFeedIndexBase? knownNearIndex,
            Func<IHasher> hasherBuilder);
        
        public async Task<SwarmFeedChunk?> TryGetFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            SwarmFeedIndexBase index,
            IHasher hasher,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunkStore, nameof(chunkStore));
            
            var hash = BuildHash(index, hasher);
            
            var chunk = await chunkStore.TryGetAsync(hash, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (chunk == null)
                return null; 
            
            return new SwarmFeedChunk(index, chunk.Data.ToArray(), hash);
        }
        
        // Static methods.
        public static byte[] BuildChunkPayload(byte[] payload, ulong? timestamp = null)
        {
            ArgumentNullException.ThrowIfNull(payload, nameof(payload));

            if (payload.Length > SwarmFeedChunk.MaxPayloadSize)
                throw new ArgumentOutOfRangeException(nameof(payload),
                    $"Payload can't be longer than {SwarmFeedChunk.MaxPayloadSize} bytes");

            var chunkData = new byte[SwarmFeedChunk.TimeStampSize + payload.Length];
            
            byte[] timestampByteArray;
            if (timestamp.HasValue)
            {
                timestampByteArray = new byte[SwarmFeedChunk.TimeStampSize];
                BinaryPrimitives.WriteUInt64BigEndian(timestampByteArray, timestamp.Value);
            }
            else
            {
                timestampByteArray = DateTimeOffset.UtcNow.ToUnixTimeSecondsByteArray();
            }
            timestampByteArray.CopyTo(chunkData, 0);
            payload.CopyTo(chunkData, SwarmFeedChunk.TimeStampSize);

            return chunkData;
        }
        
        public static SwarmHash BuildHash(EthAddress owner, byte[] topic, SwarmFeedIndexBase index, IHasher hasher) =>
            BuildHash(owner, BuildIdentifier(topic, index, hasher), hasher);

        public static SwarmHash BuildHash(EthAddress owner, byte[] identifier, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));

            if (identifier.Length != IdentifierSize)
                throw new ArgumentOutOfRangeException(nameof(identifier), "Invalid identifier length");
            
            return hasher.ComputeHash([identifier, owner.ToReadOnlyMemory()]);
        }

        public static byte[] BuildIdentifier(byte[] topic, SwarmFeedIndexBase index, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(index, nameof(index));
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));

            if (topic.Length != TopicSize)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");

            return hasher.ComputeHash([topic, index.MarshalBinary()]);
        }
    }
}