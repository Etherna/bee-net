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
using Etherna.BeeNet.Models;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public delegate IHasherPipeline BuildHasherPipeline(bool readOnlyPipeline);
    
    public class MantarayManifest(
        BuildHasherPipeline hasherBuilder,
        MantarayNode rootNode)
        : IReadOnlyMantarayManifest
    {
        // Consts.
        public static readonly string RootPath = SwarmAddress.Separator.ToString();

        // Constructors.
        public MantarayManifest(
            BuildHasherPipeline hasherBuilder,
            ushort compactLevel)
            : this(hasherBuilder,
                new MantarayNode(
                    compactLevel,
                    compactLevel > 0
                        ? null //auto-generate on hash building
                        : XorEncryptKey.Empty))
        { }

        // Properties.
        public IReadOnlyMantarayNode RootNode => rootNode;

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));

            rootNode.Add(path, entry);
        }

        public async Task<SwarmChunkReference> GetHashAsync()
        {
            await rootNode.ComputeHashAsync(hasherBuilder).ConfigureAwait(false);
            return new SwarmChunkReference(rootNode.Hash, null, false);
        }
    }
}