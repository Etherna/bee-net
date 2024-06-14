// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Models;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class MantarayManifest : IReadOnlyMantarayManifest
    {
        // Consts.
        public const string RootPath = "/";
        
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