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

using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Models;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class MantarayManifest : IReadOnlyMantarayManifest
    {
        // Consts.
        public static readonly string RootPath = SwarmAddress.Separator.ToString();
        
        // Fields.
        private readonly Func<IHasherPipeline> hasherBuilder;
        private readonly MantarayNode _rootNode;

        // Constructors.
        public MantarayManifest(
            Func<IHasherPipeline> hasherBuilder,
            bool isEncrypted)
            : this(hasherBuilder,
                new MantarayNode(isEncrypted
                    ? null //auto-generate random on hash building
                    : XorEncryptKey.Empty))
        { }

        public MantarayManifest(
            Func<IHasherPipeline> hasherBuilder,
            MantarayNode rootNode)
        {
            this.hasherBuilder = hasherBuilder;
            _rootNode = rootNode;
        }
        
        // Properties.
        public IReadOnlyMantarayNode RootNode => _rootNode;

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));

            _rootNode.Add(path, entry);
        }

        public async Task<SwarmHash> GetHashAsync()
        {
            await _rootNode.ComputeHashAsync(hasherBuilder).ConfigureAwait(false);
            return _rootNode.Hash;
        }
    }
}