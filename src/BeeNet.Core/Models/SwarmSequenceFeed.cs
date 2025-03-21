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

using Etherna.BeeNet.Stores;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class SwarmSequenceFeed(EthAddress owner, byte[] topic)
        : SwarmFeedBase(owner, topic)
    {
        // Properties.
        public override SwarmFeedType Type => SwarmFeedType.Sequence;

        // Methods.
        public override Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            byte[] contentPayload,
            SwarmFeedIndexBase? knownNearIndex)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            return BuildNextFeedChunkAsync(chunkStore, contentPayload, knownNearIndex as SwarmSequenceFeedIndex);
        }
        
        public Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IReadOnlyChunkStore chunkStore,
            byte[] contentPayload,
            SwarmSequenceFeedIndex? knownNearIndex)
        {
            throw new NotImplementedException();
        }

        public override Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            DateTimeOffset at,
            SwarmFeedIndexBase? knownNearIndex)
        {
            if (knownNearIndex is not (null or SwarmSequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            return TryFindFeedAtAsync(chunkStore, at, knownNearIndex as SwarmSequenceFeedIndex);
        }

        public Task<SwarmFeedChunk?> TryFindFeedAtAsync(
            IReadOnlyChunkStore chunkStore,
            DateTimeOffset at,
            SwarmSequenceFeedIndex? knownNearIndex)
        {
            throw new NotImplementedException();
        }
    }
}