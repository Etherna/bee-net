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
using Etherna.BeeNet.Stores;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmSequenceFeedChunk : SwarmFeedChunkBase
    {
        // Consts.
        public const int LegacyTimeStampSize = sizeof(ulong);
        
        // Internal constructors.
        internal SwarmSequenceFeedChunk(SwarmSoc soc, SwarmFeedIndexBase index, SwarmFeedTopic topic) :
            base(topic, index, soc.Identifier, soc.Owner, soc.InnerChunk, soc.Signature)
        { }

        internal SwarmSequenceFeedChunk(
            SwarmFeedTopic topic,
            SwarmSequenceFeedIndex index,
            SwarmSocIdentifier identifier,
            EthAddress owner,
            SwarmCac innerChunk,
            SwarmSocSignature? signature) :
            base(topic, index, identifier, owner, innerChunk, signature)
        { }
        
        // Static builders.
        public static SwarmFeedChunkBase BuildFromSoc(SwarmSoc soc, SwarmFeedIndexBase index, SwarmFeedTopic topic)
        {
            ArgumentNullException.ThrowIfNull(soc);
            return new SwarmSequenceFeedChunk(soc, index, topic);
        }

        public static SwarmSequenceFeedChunk BuildNew(
            SwarmSequenceFeed feed,
            SwarmSequenceFeedIndex index,
            ReadOnlyMemory<byte> data,
            SwarmChunkBmt swarmChunkBmt)
        {
            ArgumentNullException.ThrowIfNull(feed);
            return BuildNew(feed.Topic, index, feed.Owner, data, swarmChunkBmt);
        }

        public static SwarmSequenceFeedChunk BuildNew(
            SwarmFeedTopic topic,
            SwarmSequenceFeedIndex index,
            EthAddress owner,
            ReadOnlyMemory<byte> data,
            SwarmChunkBmt swarmChunkBmt)
        {
            ArgumentNullException.ThrowIfNull(swarmChunkBmt);

            return new SwarmSequenceFeedChunk(
                topic,
                index,
                BuildIdentifier(topic, index, swarmChunkBmt.Hasher),
                owner,
                BuildInnerChunk(data, swarmChunkBmt),
                null);
        }

        // Properties.
        public ReadOnlyMemory<byte> Data => FeedPayload;

        // Methods.
        public override Task<SwarmCac> UnwrapDataChunkAsync(
            bool resolveLegacyPayload,
            SwarmChunkBmt swarmChunkBmt,
            IChunkStore? chunkStore = null) =>
            UnwrapDataChunkAsync(resolveLegacyPayload, chunkStore);
        
        public async Task<SwarmCac> UnwrapDataChunkAsync(
            bool resolveLegacyPayload,
            IChunkStore? chunkStore = null)
        {
            if (resolveLegacyPayload && chunkStore == null)
                throw new ArgumentNullException(nameof(chunkStore), "Legacy payload resolution needs a chunk store.");
            
            // Check if is legacy payload with possible lengths.
            if (resolveLegacyPayload &&
                FeedPayload.Length is LegacyTimeStampSize + SwarmHash.HashSize or   // unencrypted ref
                                      LegacyTimeStampSize + SwarmHash.HashSize * 2) // encrypted ref
            {
                var hash = new SwarmHash(FeedPayload[LegacyTimeStampSize..]);
#pragma warning disable CA1062
                var chunk = await chunkStore!.GetAsync(hash).ConfigureAwait(false);
#pragma warning restore CA1062
                if (chunk is not SwarmCac cac)
                    throw new SwarmChunkTypeException(
                        chunk,
                        $"Legacy referenced chunk {hash} is not a Content Addressed Chunk");
                return cac;
            }

            return InnerChunk;
        }
        
        // Static methods.
        public static SwarmCac BuildInnerChunk(
            ReadOnlyMemory<byte> spanData,
            SwarmChunkBmt swarmChunkBmt)
        {
            ArgumentNullException.ThrowIfNull(swarmChunkBmt);
            
            var innerChunkHash = swarmChunkBmt.Hash(spanData);
            var innerChunk = new SwarmCac(innerChunkHash, spanData);
            return innerChunk;
        }
    }
}
