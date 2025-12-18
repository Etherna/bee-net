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
    public sealed class ReferencedMantarayManifest(ReferencedMantarayNode rootNode)
        : MantarayManifestBase
    {
        // Static builders.
        public static ReferencedMantarayManifest BuildNew(
            SwarmReference rootReference,
            IReadOnlyChunkStore chunkStore,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback)
        {
            var node = new ReferencedMantarayNode(
                rootReference,
                chunkStore,
                redundancyStrategy,
                redundancyStrategyFallback,
                null,
                NodeType.Edge);
            return new ReferencedMantarayManifest(node);
        }

        public static ReferencedMantarayManifest BuildNew(
            SwarmCac rootChunk,
            SwarmReference rootChunkReference,
            IReadOnlyChunkStore chunkStore,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback)
        {
            var node = new ReferencedMantarayNode(
                rootChunk,
                rootChunkReference,
                chunkStore,
                redundancyStrategy,
                redundancyStrategyFallback,
                null,
                NodeType.Edge);
            return new ReferencedMantarayManifest(node);
        }

        // Properties.
        public override IReadOnlyMantarayNode RootNode => rootNode;

        // Methods.
        public override Task<SwarmReference> GetReferenceAsync(Hasher hasher) =>
            Task.FromResult(RootNode.Reference);
    }
}