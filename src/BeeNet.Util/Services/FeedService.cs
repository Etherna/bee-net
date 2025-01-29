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
    public class FeedService
        : IFeedService
    {
        // Consts.
        public const string FeedMetadataEntryOwner = "swarm-feed-owner";
        public const string FeedMetadataEntryTopic = "swarm-feed-topic";
        public const string FeedMetadataEntryType  = "swarm-feed-type";
        
        // Methods.
        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public async Task<SwarmFeedBase?> TryDecodeFeedManifestAsync(ReferencedMantarayManifest manifest)
        {
            ArgumentNullException.ThrowIfNull(manifest, nameof(manifest));
            
            var metadata = await manifest.GetResourceMetadataAsync(MantarayManifest.RootPath).ConfigureAwait(false);
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

                return Enum.Parse<FeedType>(strType, true) switch
                {
                    FeedType.Epoch => new EpochFeed(owner, topic),
                    FeedType.Sequence => new SequenceFeed(owner, topic),
                    _ => throw new InvalidOperationException()
                };
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<UploadEvaluationResult> UploadFeedManifestAsync(
            byte[] account,
            byte[] topic,
            FeedType feedType,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null)
        {
            // Init.
            chunkStore ??= new FakeChunkStore();
            postageStampIssuer ??= new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            var postageStamper = new PostageStamper(
                new FakeSigner(),
                postageStampIssuer,
                new MemoryStampStore());

            // Create manifest.
            var feedManifest = new MantarayManifest(
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    RedundancyLevel.None,
                    false,
                    0,
                    null),
                false);
            
            feedManifest.Add(
                MantarayManifest.RootPath,
                ManifestEntry.NewDirectory(new Dictionary<string, string>
                {
                    [FeedMetadataEntryOwner] = account.ToHex(),
                    [FeedMetadataEntryTopic] = topic.ToHex(),
                    [FeedMetadataEntryType] = feedType.ToString()
                }));

            var chunkHashingResult = await feedManifest.GetHashAsync().ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                chunkHashingResult,
                0,
                postageStampIssuer);
        }
    }
}