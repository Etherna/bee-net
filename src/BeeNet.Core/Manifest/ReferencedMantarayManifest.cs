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

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class ReferencedMantarayManifest(
        IReadOnlyChunkStore chunkStore,
        SwarmHash rootHash,
        bool useChunkStoreCache = false)
        : IReadOnlyMantarayManifest
    {
        // Fields.
        private readonly ReferencedMantarayNode _rootNode = new(chunkStore, rootHash, null, NodeType.Edge, useChunkStoreCache);

        // Properties.
        public IReadOnlyMantarayNode RootNode => _rootNode;

        // Methods.
        public Task<SwarmHash> GetHashAsync() => Task.FromResult(RootNode.Hash);

        public async Task<IReadOnlyDictionary<string, string>> GetResourceMetadataAsync(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            
            if (!_rootNode.IsDecoded)
                await _rootNode.DecodeFromChunkAsync().ConfigureAwait(false);

            return await RootNode.GetResourceMetadataAsync(path.TrimStart(SwarmAddress.Separator)).ConfigureAwait(false);
        }

        public async Task<SwarmChunkReference> ResolveAddressToChunkReferenceAsync(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            
            if (!_rootNode.IsDecoded)
                await _rootNode.DecodeFromChunkAsync().ConfigureAwait(false);

            return await _rootNode.ResolveChunkReferenceAsync(
                path == SwarmAddress.Separator.ToString() ?
                    path :
                    path.TrimStart(SwarmAddress.Separator)).ConfigureAwait(false);
        }
    }
}