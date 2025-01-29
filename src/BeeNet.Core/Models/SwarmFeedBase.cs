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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Hashing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public abstract class SwarmFeedBase
    {
        // Fields.
        protected readonly byte[] _owner;
        protected readonly byte[] _topic;

        // Constructors.
        protected SwarmFeedBase(byte[] owner, byte[] topic)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));

            if (owner.Length != SwarmFeedChunk.OwnerAccountSize)
                throw new ArgumentOutOfRangeException(nameof(owner), "Invalid owner account length");
            if (topic.Length != SwarmFeedChunk.TopicSize)
                throw new ArgumentOutOfRangeException(nameof(topic), "Invalid topic length");
            
            _owner = owner;
            _topic = topic;
        }

        // Properties.
        public ReadOnlyMemory<byte> Owner => _owner.AsMemory();
        public ReadOnlyMemory<byte> Topic => _topic.AsMemory();
        public abstract FeedType Type { get; }
        
        // Methods.
        public abstract Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IBeeClient beeClient,
            byte[] contentPayload,
            FeedIndexBase? knownNearIndex);

        /// <summary>
        /// Try to find feed at a given time
        /// </summary>
        /// <param name="at">The time to search</param>
        /// <param name="knownNearIndex">Another known existing index, near to looked time. Helps to perform lookup quicker</param>
        /// <returns>The found feed chunk, or null</returns>
        public abstract Task<SwarmFeedChunk?> TryFindFeedAsync(
            IBeeClient beeClient,
            DateTimeOffset at,
            FeedIndexBase? knownNearIndex);
        
        public async Task<SwarmFeedChunk?> TryGetFeedChunkAsync(
            IBeeClient beeClient,
            FeedIndexBase index)
        {
            ArgumentNullException.ThrowIfNull(beeClient, nameof(beeClient));
            
            var hash = SwarmFeedChunk.BuildHash(_owner, _topic, index, new Hasher());
            try
            {
                using var chunkStream = await beeClient.GetChunkStreamAsync(hash).ConfigureAwait(false);
                using var chunkMemoryStream = new MemoryStream();
                await chunkStream.CopyToAsync(chunkMemoryStream).ConfigureAwait(false);
                return new SwarmFeedChunk(index, chunkMemoryStream.ToArray(), hash);
            }
            catch (BeeNetApiException)
            {
                return null;
            }
        }
    }
}