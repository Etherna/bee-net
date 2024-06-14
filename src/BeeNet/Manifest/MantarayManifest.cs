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
using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class MantarayManifest
    {
        // Consts.
        public const string RootPath = "/";
        
        // Fields.
        private readonly Func<IHasherPipeline> hasherBuilder1;

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
            hasherBuilder1 = hasherBuilder;
            RootNode = rootNode;
        }
        
        // Builder methods.
        public static async Task<MantarayManifest> CreateFromStoredChunkAsync(
            IChunkStore chunkStore,
            SwarmHash rootHash,
            Func<IHasherPipeline> hasherBuilder)
        {
            ArgumentNullException.ThrowIfNull(chunkStore, nameof(chunkStore));
            
            var rootChunk = await chunkStore.GetAsync(rootHash).ConfigureAwait(false);
            return new MantarayManifest(hasherBuilder, new StoredMantarayNode(rootChunk, chunkStore));
        }
        
        // Properties.
        public MantarayNode RootNode { get; }

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));

            RootNode.Add(path, entry);
        }

        public async Task<SwarmHash> GetHashAsync()
        {
            await RootNode.ComputeHashAsync(hasherBuilder1).ConfigureAwait(false);
            return RootNode.Hash;
        }

        public Task<Stream> GetResourceStreamAsync(SwarmAddress address)
        {
            throw new NotImplementedException();
        }
    }
}