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

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Chunks
{
    public class ChunkTraverser(
        IReadOnlyChunkStore chunkStore)
    {
        // Public delegates.
        public delegate Task OnChunkFoundAsync(SwarmCac chunk, SwarmShardReference shardReference);
        public delegate Task OnInvalidChunkFoundAsync(SwarmChunk chunk, SwarmShardReference shardReference);
        public delegate Task OnChunkNotFoundAsync(SwarmShardReference shardReference);
        
        // Methods.
        /// <summary>
        /// Try to traverse as from a mantaray manifest, and if fails as a data chunk
        /// </summary>
        /// <param name="rootReference">Root traversing chunk</param>
        public async Task TraverseAsync(
            SwarmReference rootReference,
            OnChunkFoundAsync? onChunkFound,
            OnInvalidChunkFoundAsync? onInvalidChunkFound,
            OnChunkNotFoundAsync? onChunkNotFound,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback,
            bool includeParities,
            CancellationToken cancellationToken = default)
        {
            // Upgrade redundancy strategy from None to Data.
            if (redundancyStrategy == RedundancyStrategy.None)
                redundancyStrategy = RedundancyStrategy.Data;
            
            // Identify if is manifest root chunk.
            var isManifestChunk = false;
            try
            {
                var manifest = await ReferencedMantarayManifest.BuildNewAsync(
                    rootReference,
                    chunkStore,
                    redundancyLevel,
                    redundancyStrategy,
                    redundancyStrategyFallback).ConfigureAwait(false);
                await manifest.RootNode.OnVisitingAsync(cancellationToken).ConfigureAwait(false);
                isManifestChunk = true;
            }
            catch (InvalidOperationException) //in case it's not a manifest
            { }
            catch (KeyNotFoundException) //in case root chunk is not found
            {
                onChunkNotFound?.Invoke(new SwarmShardReference(rootReference, false));
                return;
            }

            // Traverse with identified chunk type.
            if (isManifestChunk)
                await TraverseFromMantarayManifestRootAsync(
                    rootReference,
                    onChunkFound,
                    onInvalidChunkFound,
                    onChunkNotFound,
                    redundancyLevel,
                    redundancyStrategy,
                    redundancyStrategyFallback,
                    includeParities,
                    cancellationToken).ConfigureAwait(false);
            else
                await TraverseFromDataChunkAsync(
                    rootReference,
                    onChunkFound,
                    onInvalidChunkFound,
                    onChunkNotFound,
                    redundancyLevel,
                    redundancyStrategy,
                    redundancyStrategyFallback,
                    includeParities,
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task TraverseFromDataChunkAsync(
            SwarmReference reference,
            OnChunkFoundAsync? onChunkFound,
            OnInvalidChunkFoundAsync? onInvalidChunkFound,
            OnChunkNotFoundAsync? onChunkNotFound,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback,
            bool includeParities,
            CancellationToken cancellationToken = default)
        {
            onChunkFound ??= (_, _) => Task.CompletedTask;
            onInvalidChunkFound ??= (_, _) => Task.CompletedTask;
            onChunkNotFound ??= _ => Task.CompletedTask;

            // Upgrade redundancy strategy from None to Data.
            if (redundancyStrategy == RedundancyStrategy.None)
                redundancyStrategy = RedundancyStrategy.Data;

            // Read as data or intermediate chunk.
            await TraverseDataHelperAsync(
                reference,
                [],
                onChunkFound,
                onInvalidChunkFound,
                onChunkNotFound,
                redundancyLevel,
                redundancyStrategy,
                redundancyStrategyFallback,
                includeParities,
                cancellationToken).ConfigureAwait(false);
        }
        
        public async Task TraverseFromMantarayManifestRootAsync(
            SwarmReference rootReference,
            OnChunkFoundAsync? onChunkFound,
            OnInvalidChunkFoundAsync? onInvalidChunkFound,
            OnChunkNotFoundAsync? onChunkNotFound,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback,
            bool includeParities,
            CancellationToken cancellationToken = default)
        {
            onChunkFound ??= (_, _) => Task.CompletedTask;
            onInvalidChunkFound ??= (_, _) => Task.CompletedTask;
            onChunkNotFound ??= _ => Task.CompletedTask;
            
            // Upgrade redundancy strategy from None to Data.
            if (redundancyStrategy == RedundancyStrategy.None)
                redundancyStrategy = RedundancyStrategy.Data;

            // Read as manifest.
            var manifest = await ReferencedMantarayManifest.BuildNewAsync(
                rootReference,
                chunkStore,
                redundancyLevel,
                redundancyStrategy,
                redundancyStrategyFallback).ConfigureAwait(false);
            await TraverseMantarayNodeHelperAsync(
                (ReferencedMantarayNode)manifest.RootNode,
                [],
                onChunkFound,
                onInvalidChunkFound,
                onChunkNotFound,
                redundancyLevel,
                redundancyStrategy,
                redundancyStrategyFallback,
                includeParities,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task TraverseFromMantarayNodeChunkAsync(
            SwarmReference nodeReference,
            NodeType nodeTypeFlags,
            OnChunkFoundAsync? onChunkFound,
            OnInvalidChunkFoundAsync? onInvalidChunkFound,
            OnChunkNotFoundAsync? onChunkNotFound,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback,
            bool includeParities,
            CancellationToken cancellationToken = default)
        {
            onChunkFound ??= (_, _) => Task.CompletedTask;
            onInvalidChunkFound ??= (_, _) => Task.CompletedTask;
            onChunkNotFound ??= _ => Task.CompletedTask;

            // Upgrade redundancy strategy from None to Data.
            if (redundancyStrategy == RedundancyStrategy.None)
                redundancyStrategy = RedundancyStrategy.Data;

            // Read as manifest node.
            var manifestNode = await ReferencedMantarayNode.BuildNewAsync(
                nodeReference,
                chunkStore,
                redundancyLevel,
                redundancyStrategy,
                redundancyStrategyFallback,
                null,
                nodeTypeFlags,
                cancellationToken).ConfigureAwait(false);
            
            await TraverseMantarayNodeHelperAsync(
                manifestNode,
                [],
                onChunkFound,
                onInvalidChunkFound,
                onChunkNotFound,
                redundancyLevel,
                redundancyStrategy,
                redundancyStrategyFallback,
                includeParities,
                cancellationToken).ConfigureAwait(false);
        }

        // Helpers.
        private async Task TraverseDataHelperAsync(
            SwarmReference rootReference,
            HashSet<SwarmReference> visitedReferences,
            OnChunkFoundAsync onChunkFound,
            OnInvalidChunkFoundAsync onInvalidChunkFound,
            OnChunkNotFoundAsync onChunkNotFound,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback,
            bool includeParities,
            CancellationToken cancellationToken)
        {
            Queue<(SwarmCac Chunk, SwarmReference Reference)> chunkRefPairs = [];
            var hasher = new Hasher();
            
            // Try resolve root chunk from reference.
            var rootChunkStore = redundancyLevel == RedundancyLevel.None ?
                chunkStore :
                new ReplicaResolverChunkStore(chunkStore, redundancyLevel, hasher);

            var rootChunk = await rootChunkStore.TryGetAsync(
                rootReference.Hash,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            var rootShardReference = new SwarmShardReference(rootReference, false);
            if (rootChunk == null)
            {
                await onChunkNotFound(rootShardReference).ConfigureAwait(false);
                return;
            }
            if (rootChunk is not SwarmCac rootCac) //soc are not supported
            {
                await onInvalidChunkFound(rootChunk, rootShardReference).ConfigureAwait(false);
                return;
            }
            await onChunkFound(rootCac, rootShardReference).ConfigureAwait(false);
            
            // Run levels iteration starting from root chunk.
            chunkRefPairs.Enqueue((rootCac, rootReference));
            while (chunkRefPairs.Count > 0)
            {
                var (chunk, reference) = chunkRefPairs.Dequeue();
                
                // Try to add the reference to visited refs. Continue if already present.
                if (!visitedReferences.Add(reference))
                    continue;
                
                // Decode chunk.
                var decodedChunk = chunk.Decode(reference, hasher);
                
                // Skip iteration on data chunks.
                if (decodedChunk.IsDataChunk)
                    continue;
                
                // If intermediate chunk, extract child references.
                var childReferences = ((SwarmDecodedIntermediateCac)decodedChunk).ChildReferences;
                    
                // Run fetch and recover with parity asynchronously.
                var decoder = new ChunkParityDecoder(childReferences, chunkStore);
                await decoder.TryFetchAndRecoverAsync(
                    redundancyStrategy,
                    redundancyStrategyFallback,
                    forceFetchAllChunks: true,
                    forceRecoverParities: includeParities,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                
                // Try to iterate on child chunks from decoder.
                foreach (var childReference in childReferences.Where(r => !r.IsParity || includeParities))
                {
                    // Try to get the chunk.
                    var childChunk = decoder.TryGetChunk(childReference.Reference.Hash);
                    if (childChunk == null)
                    {
                        await onChunkNotFound(childReference).ConfigureAwait(false);
                    }
                    else
                    {
                        await onChunkFound(childChunk, childReference).ConfigureAwait(false);
                        chunkRefPairs.Enqueue((childChunk, childReference.Reference));
                    }
                }
            }
        }
        
        private async Task TraverseMantarayNodeHelperAsync(
            ReferencedMantarayNode manifestNode,
            HashSet<SwarmReference> visitedReferences,
            OnChunkFoundAsync onChunkFound,
            OnInvalidChunkFoundAsync onInvalidChunkFound,
            OnChunkNotFoundAsync onChunkNotFound,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy, 
            bool redundancyStrategyFallback,
            bool includeParities,
            CancellationToken cancellationToken)
        {
            visitedReferences.Add(manifestNode.Reference);
            
            // Try decode manifest.
            try
            {
                await manifestNode.OnVisitingAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (KeyNotFoundException)
            {
                await onChunkNotFound(
                    new SwarmShardReference(manifestNode.Reference.Hash, false)).ConfigureAwait(false);
                return;
            }
            await onChunkFound(
                manifestNode.Chunk,
                new SwarmShardReference(manifestNode.Reference, false)).ConfigureAwait(false);
            
            // Traverse forks.
            foreach (var fork in manifestNode.Forks.Values)
            {
                //skip already visited chunks
                if (visitedReferences.Contains(fork.Node.Reference))
                    continue;
                
                await TraverseMantarayNodeHelperAsync(
                    (ReferencedMantarayNode)fork.Node,
                    visitedReferences,
                    onChunkFound,
                    onInvalidChunkFound,
                    onChunkNotFound,
                    redundancyLevel,
                    redundancyStrategy,
                    redundancyStrategyFallback,
                    includeParities,
                    cancellationToken).ConfigureAwait(false);
            }
            
            // Traverse data.
            if (manifestNode.EntryReference.HasValue &&
                !SwarmReference.IsZero(manifestNode.EntryReference.Value) &&
                !visitedReferences.Contains(manifestNode.EntryReference.Value)) //skip already visited chunks
                await TraverseDataHelperAsync(
                    manifestNode.EntryReference.Value,
                    visitedReferences,
                    onChunkFound,
                    onInvalidChunkFound,
                    onChunkNotFound,
                    redundancyLevel,
                    redundancyStrategy,
                    redundancyStrategyFallback,
                    includeParities,
                    cancellationToken).ConfigureAwait(false);
        }
    }
}