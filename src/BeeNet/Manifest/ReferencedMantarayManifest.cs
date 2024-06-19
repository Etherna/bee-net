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

using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class ReferencedMantarayManifest : IReadOnlyMantarayManifest
    {
        // Fields.
        private readonly ReferencedMantarayNode _rootNode;

        // Constructors.
        public ReferencedMantarayManifest(
            IChunkStore chunkStore,
            SwarmHash rootHash)
        {
            _rootNode = new ReferencedMantarayNode(chunkStore, rootHash, null, NodeType.Edge);
        }
        
        // Properties.
        public IReadOnlyMantarayNode RootNode => _rootNode;

        // Methods.
        public Task<SwarmHash> GetHashAsync() => Task.FromResult(RootNode.Hash);

        public async Task<IReadOnlyDictionary<string, string>> GetResourceMetadataAsync(SwarmAddress address)
        {
            if (!_rootNode.IsDecoded)
                await _rootNode.DecodeFromChunkAsync().ConfigureAwait(false);

            return await RootNode.GetResourceMetadataAsync(
                address.RelativePath?.ToString() ?? "").ConfigureAwait(false);
        }

        public async Task<SwarmHash> ResolveResourceHashAsync(SwarmAddress address)
        {
            if (!_rootNode.IsDecoded)
                await _rootNode.DecodeFromChunkAsync().ConfigureAwait(false);

            return await RootNode.ResolveResourceHashAsync(
                address.RelativePath?.ToString() ?? "").ConfigureAwait(false);
        }
    }
}