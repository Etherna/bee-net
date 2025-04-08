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
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public abstract class SwarmFeedBase(EthAddress owner, SwarmFeedTopic topic)
    {
        // Consts.
        protected static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
        
        // Properties.
        public EthAddress Owner { get; } = owner;
        public SwarmFeedTopic Topic { get; } = topic;
        public abstract SwarmFeedType Type { get; }
        
        // Methods.
        public SwarmHash BuildHash(SwarmFeedIndexBase index, IHasher hasher) =>
            SwarmFeedChunkBase.BuildHash(Topic, index, Owner, hasher);

        public SwarmSocIdentifier BuildIdentifier(SwarmFeedIndexBase index, IHasher hasher) =>
            SwarmFeedChunkBase.BuildIdentifier(Topic, index, hasher);
        
        public abstract Task<SwarmFeedChunkBase> BuildNextFeedChunkAsync(
            ReadOnlyMemory<byte> contentData,
            SwarmFeedIndexBase? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            ISwarmChunkBmt chunkBmt,
            DateTimeOffset? timestamp = null);

        /// <summary>
        /// Try to find feed at a given time
        /// </summary>
        /// <param name="at">The time to search</param>
        /// <param name="knownNearIndex">Another known existing index, near to looked time. Helps to perform lookup quicker</param>
        /// <param name="chunkStore">The chunk store</param>
        /// <returns>The found feed chunk, or null</returns>
        public abstract Task<SwarmFeedChunkBase?> TryFindFeedChunkAtAsync(
            long at,
            SwarmFeedIndexBase? knownNearIndex,
            IReadOnlyChunkStore chunkStore,
            IHasher hasher);

        public async Task<SwarmFeedChunkBase?> TryGetFeedChunkAsync(
            SwarmFeedIndexBase index,
            IReadOnlyChunkStore chunkStore,
            IHasher hasher,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunkStore, nameof(chunkStore));
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));

            var hash = BuildHash(index, hasher);

            var chunk = await chunkStore.TryGetAsync(hash, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (chunk is not SwarmFeedChunkBase feedChunk)
                return null;

            return feedChunk;
        }
    }
}