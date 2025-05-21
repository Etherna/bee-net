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
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public sealed class ReferencedMantarayManifest : MantarayManifestBase
    {
        // Fields.
        private readonly ReferencedMantarayNode rootNode;

        // Constructors.
        public ReferencedMantarayManifest(
            IReadOnlyChunkStore chunkStore,
            SwarmHash rootHash,
            bool useChunkStoreCache = false)
        {
            rootNode = new ReferencedMantarayNode(chunkStore, rootHash, null, NodeType.Edge, useChunkStoreCache);
        }

        public ReferencedMantarayManifest(
            IReadOnlyChunkStore chunkStore,
            SwarmCac rootChunk,
            bool useChunkStoreCache = false)
        {
            rootNode = new ReferencedMantarayNode(chunkStore, rootChunk, null, NodeType.Edge, useChunkStoreCache);
        }

        // Properties.
        public override IReadOnlyMantarayNode RootNode => rootNode;

        // Methods.
        public override Task<SwarmChunkReference> GetHashAsync(Hasher hasher) =>
            Task.FromResult(new SwarmChunkReference(RootNode.Hash, null, false));
    }
}