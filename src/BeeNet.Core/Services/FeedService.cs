﻿// Copyright 2021-present Etherna SA
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
using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Hashing.Signer;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public class FeedService : IFeedService
    {
        // Consts.
        public const string FeedMetadataEntryOwner = "swarm-feed-owner";
        public const string FeedMetadataEntryTopic = "swarm-feed-topic";
        public const string FeedMetadataEntryType  = "swarm-feed-type";
        
        // Methods.
        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public async Task<SwarmFeedBase?> TryDecodeFeedManifestAsync(
            ReferencedMantarayManifest manifest)
        {
            ArgumentNullException.ThrowIfNull(manifest, nameof(manifest));
            
            var metadata = (await manifest.GetMetadataAsync(
                MantarayManifestBase.RootPath,
                ManifestPathResolver.IdentityResolver).ConfigureAwait(false)).Result;
            if (!metadata.TryGetValue(FeedMetadataEntryOwner, out var hexOwner))
                return null;
            if (!metadata.TryGetValue(FeedMetadataEntryTopic, out var hexTopic))
                return null;
            if (!metadata.TryGetValue(FeedMetadataEntryType, out var strType))
                return null;

            try
            {
                var owner = hexOwner.HexToByteArray();
                var topic = hexTopic.HexToByteArray();

                return Enum.Parse<SwarmFeedType>(strType, true) switch
                {
                    SwarmFeedType.Epoch => new SwarmEpochFeed(owner, topic),
                    SwarmFeedType.Sequence => new SwarmSequenceFeed(owner, topic),
                    _ => throw new InvalidOperationException()
                };
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<SwarmChunkReference> UploadFeedManifestAsync(
            SwarmFeedBase swarmFeed,
            Hasher hasher,
            ushort compactLevel = 0,
            IPostageStamper? postageStamper = null,
            IChunkStore? chunkStore = null)
        {
            ArgumentNullException.ThrowIfNull(swarmFeed, nameof(swarmFeed));
            
            // Init.
            chunkStore ??= new FakeChunkStore();
            postageStamper ??= new PostageStamper(
                new FakeSigner(),
                new PostageStampIssuer(PostageBatch.MaxDepthInstance),
                new MemoryStampStore());

            // Create manifest.
            var feedManifest = new WritableMantarayManifest(
                readOnlyPipeline => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    RedundancyLevel.None,
                    false,
                    0,
                    null,
                    readOnlyPipeline),
                compactLevel);

            feedManifest.Add(
                MantarayManifestBase.RootPath,
                ManifestEntry.NewFile(
                    SwarmHash.Zero,
                    new Dictionary<string, string>
                    {
                        [FeedMetadataEntryOwner] = swarmFeed.Owner.ToByteArray().ToHex(),
                        [FeedMetadataEntryTopic] = swarmFeed.Topic.ToString(),
                        [FeedMetadataEntryType] = swarmFeed.Type.ToString()
                    }));

            return await feedManifest.GetHashAsync(hasher).ConfigureAwait(false);
        }
    }
}