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

using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class SequenceFeed : SwarmFeedBase
    {
        // Constructors.
        public SequenceFeed(string owner, byte[] topic)
            : base(owner.HexToByteArray(), topic)
        { }

        public SequenceFeed(byte[] owner, byte[] topic)
            : base(owner, topic)
        { }

        // Properties.
        public override FeedType Type => FeedType.Sequence;

        // Methods.
        public override Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IBeeClient beeClient,
            byte[] contentPayload,
            FeedIndexBase? knownNearIndex)
        {
            if (knownNearIndex is not (null or SequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            return BuildNextFeedChunkAsync(beeClient, contentPayload, knownNearIndex as SequenceFeedIndex);
        }
        
        public Task<SwarmFeedChunk> BuildNextFeedChunkAsync(
            IBeeClient beeClient,
            byte[] contentPayload,
            SequenceFeedIndex? knownNearIndex)
        {
            throw new NotImplementedException();
        }

        public override Task<SwarmFeedChunk?> TryFindFeedAsync(
            IBeeClient beeClient,
            DateTimeOffset at,
            FeedIndexBase? knownNearIndex)
        {
            if (knownNearIndex is not (null or SequenceFeedIndex))
                throw new ArgumentException("Feed index bust be null or sequence index", nameof(knownNearIndex));
            return TryFindFeedAsync(beeClient, at, knownNearIndex as SequenceFeedIndex);
        }

        public Task<SwarmFeedChunk?> TryFindFeedAsync(
            IBeeClient beeClient,
            DateTimeOffset at,
            SequenceFeedIndex? knownNearIndex)
        {
            throw new NotImplementedException();
        }
    }
}