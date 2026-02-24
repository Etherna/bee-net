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
using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public delegate IHasherPipeline BuildHasherPipeline(bool readOnlyPipeline);
    
    public sealed class WritableMantarayManifest : MantarayManifestBase
    {
        // Fields.
        private readonly BuildHasherPipeline hasherPipelineBuilder;
        private readonly WritableMantarayNode rootNode;

        // Constructors.
        public WritableMantarayManifest(
            BuildHasherPipeline hasherPipelineBuilder,
            WritableMantarayNode rootNode)
        {
            this.hasherPipelineBuilder = hasherPipelineBuilder;
            this.rootNode = rootNode;
        }
        
        public WritableMantarayManifest(
            IChunkStore chunkStore,
            IPostageStamper postageStamper,
            RedundancyLevel redundancyLevel,
            bool encrypt,
            ushort compactLevel,
            int? chunkHashingConcurrency)
        {
            /*
             * If encrypted:
             * - compact chunks and encrypt with the hashing pipeline
             * - use obfuscation with random keys (initialize key == null). Don't mine on obfuscation
             *
             * If not encrypted, but compacted:
             * - compact chunks when required using deterministic obfuscation keys
             *
             * If not encrypted and not compacted:
             * - set obfuscation key to EncryptionKey256.Zero
             */

            hasherPipelineBuilder = readOnlyPipeline => HasherPipelineBuilder.BuildNewHasherPipeline(
                chunkStore,
                postageStamper,
                redundancyLevel,
                encrypt,
                encrypt ? compactLevel : (ushort)0,
                chunkHashingConcurrency,
                readOnlyPipeline);
            rootNode = new WritableMantarayNode(
                encrypt,
                encrypt ? (ushort)0 : compactLevel,
                !encrypt && compactLevel == 0 ?
                    EncryptionKey256.Zero :
                    (EncryptionKey256?)null); //auto-generate on hash building
        }

        // Properties.
        public override IReadOnlyMantarayNode RootNode => rootNode;

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(entry);

            rootNode.Add(path, entry);
        }

        public override async Task<SwarmReference> GetReferenceAsync(Hasher hasher)
        {
            await rootNode.ComputeHashAsync(hasher, hasherPipelineBuilder).ConfigureAwait(false);
            return rootNode.Reference;
        }
    }
}