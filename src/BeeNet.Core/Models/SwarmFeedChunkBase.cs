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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public abstract class SwarmFeedChunkBase(
        SwarmFeedTopic topic,
        SwarmFeedIndexBase index,
        SwarmSocIdentifier identifier,
        EthAddress owner,
        SwarmCac innerChunk,
        SwarmSocSignature? signature)
        : SwarmSoc(identifier, owner, innerChunk, signature)
    {
        // Properties.
        public ReadOnlyMemory<byte> FeedPayload => InnerChunk.Data;
        public SwarmFeedIndexBase Index { get; } = index ?? throw new ArgumentNullException(nameof(index));
        public SwarmFeedTopic Topic { get; } = topic;

        // Methods.
        public abstract Task<SwarmCac> UnwrapDataChunkAsync(
            bool resolveLegacyPayload,
            SwarmChunkBmt swarmChunkBmt,
            IChunkStore? chunkStore = null);
        
        // Static methods.
        public static SwarmHash BuildHash(
            SwarmFeedTopic topic,
            SwarmFeedIndexBase index,
            EthAddress owner,
            Hasher hasher) =>
            BuildHash(BuildIdentifier(topic, index, hasher), owner, hasher);
        
        public static SwarmSocIdentifier BuildIdentifier(
            SwarmFeedTopic topic,
            SwarmFeedIndexBase index,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(index, nameof(index));

            return hasher.ComputeHash([topic.ToReadOnlyMemory(), index.MarshalBinary()]);
        }
    }
}
