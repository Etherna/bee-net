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
using Etherna.BeeNet.Models;
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
            ushort compactLevel)
            : this(
                hasherPipelineBuilder,
                new WritableMantarayNode(
                    compactLevel,
                    compactLevel > 0
                        ? (EncryptionKey256?)null //auto-generate on hash building
                        : EncryptionKey256.Zero))
        { }

        public WritableMantarayManifest(
            BuildHasherPipeline hasherPipelineBuilder,
            WritableMantarayNode rootNode)
        {
            this.hasherPipelineBuilder = hasherPipelineBuilder;
            this.rootNode = rootNode;
        }

        // Properties.
        public override IReadOnlyMantarayNode RootNode => rootNode;

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));

            rootNode.Add(path, entry);
        }

        public override async Task<SwarmReference> GetHashAsync(Hasher hasher)
        {
            await rootNode.ComputeHashAsync(hasher, hasherPipelineBuilder).ConfigureAwait(false);
            return new SwarmReference(rootNode.Hash, null);
        }
    }
}