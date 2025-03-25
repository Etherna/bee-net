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

using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Stores;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields")]
    public abstract class SwarmFeedBase
    {
        // Fields.
        protected readonly byte[] _topic;

        // Constructors.
        protected SwarmFeedBase(EthAddress owner, byte[] topic)
        {
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));

            if (topic.Length != SwarmFeedChunk.TopicSize)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");
            
            Owner = owner;
            _topic = topic;
        }

        // Properties.
        public EthAddress Owner { get; }
        public ReadOnlyMemory<byte> Topic => _topic.AsMemory();
        public abstract SwarmFeedType Type { get; }
        
        // Methods.
        public abstract Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            byte[] contentPayload,
            SwarmFeedIndexBase? knownNearIndex);

        /// <summary>
        /// Try to find feed at a given time
        /// </summary>
        /// <param name="chunkStore">The chunk store</param>
        /// <param name="at">The time to search</param>
        /// <param name="knownNearIndex">Another known existing index, near to looked time. Helps to perform lookup quicker</param>
        /// <returns>The found feed chunk, or null</returns>
        public abstract Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            long at,
            SwarmFeedIndexBase? knownNearIndex);
        
        public async Task<SwarmFeedChunk?> TryGetFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            SwarmFeedIndexBase index)
        {
            ArgumentNullException.ThrowIfNull(chunkStore, nameof(chunkStore));
            
            var hash = SwarmFeedChunk.BuildHash(Owner, _topic, index, new Hasher());
            
            var chunk = await chunkStore.TryGetAsync(hash).ConfigureAwait(false);
            if (chunk == null)
                return null; 
            
            return new SwarmFeedChunk(index, chunk.Data.ToArray(), hash);
        }
    }
}