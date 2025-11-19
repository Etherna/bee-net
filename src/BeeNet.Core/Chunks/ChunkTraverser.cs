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
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Chunks
{
    public class ChunkTraverser(
        IReadOnlyChunkStore chunkStore)
    {
        // Methods.
        /// <summary>
        /// Try to traverse as from a mantaray manifest, and if fails as a data chunk
        /// </summary>
        /// <param name="rootReference">Root traversing chunk</param>
        public async Task TraverseAsync(
            SwarmReference rootReference,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            // Identify if is manifest root chunk.
            var isManifestChunk = false;
            try
            {
                var manifest = new ReferencedMantarayManifest(chunkStore, rootReference);
                await manifest.RootNode.OnVisitingAsync().ConfigureAwait(false);
                isManifestChunk = true;
            }
            catch (InvalidOperationException) //in case it's not a manifest
            { }
            catch (KeyNotFoundException) //in case root chunk is not found
            {
                onChunkNotFoundAsync?.Invoke(rootReference.Hash);
                return;
            }

            // Traverse with identified chunk type.
            if (isManifestChunk)
                await TraverseFromMantarayManifestRootAsync(
                    rootReference,
                    onChunkFoundAsync,
                    onInvalidChunkFoundAsync,
                    onChunkNotFoundAsync).ConfigureAwait(false);
            else
                await TraverseFromDataChunkAsync(
                    rootReference,
                    onChunkFoundAsync,
                    onInvalidChunkFoundAsync,
                    onChunkNotFoundAsync).ConfigureAwait(false);
        }

        public async Task TraverseFromDataChunkAsync(
            SwarmReference reference,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onInvalidChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;

            // Read as data or intermediate chunk.
            await TraverseDataHelperAsync(
                reference,
                [],
                onChunkFoundAsync,
                onInvalidChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }
        
        public async Task TraverseFromMantarayManifestRootAsync(
            SwarmReference rootReference,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onInvalidChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;
            
            // Read as manifest.
            var manifest = new ReferencedMantarayManifest(chunkStore, rootReference);
            await TraverseMantarayNodeHelperAsync(
                (ReferencedMantarayNode)manifest.RootNode,
                [],
                onChunkFoundAsync,
                onInvalidChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }

        public async Task TraverseFromMantarayNodeChunkAsync(
            SwarmReference nodeReference,
            NodeType nodeTypeFlags,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onInvalidChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;

            // Read as manifest node.
            var manifestNode = new ReferencedMantarayNode(
                chunkStore,
                nodeReference,
                null,
                nodeTypeFlags,
                true);
            await TraverseMantarayNodeHelperAsync(
                manifestNode,
                [],
                onChunkFoundAsync,
                onInvalidChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }

        // Helpers.
        private async Task TraverseDataHelperAsync(
            SwarmReference rootReference,
            HashSet<SwarmReference> visitedReferences,
            Func<SwarmChunk, Task> onChunkFoundAsync,
            Func<SwarmChunk, Task> onInvalidChunkFoundAsync,
            Func<SwarmHash, Task> onChunkNotFoundAsync)
        {
            List<SwarmReference> chunkReferences = [rootReference];
            var spanBuffer = new byte[SwarmCac.SpanSize];
            var dataBuffer = new byte[SwarmCac.DataSize];
            var hasher = new Hasher();
            var referenceSize = rootReference.Size;

            while (chunkReferences.Count > 0)
            {
                var nextLevelChunkRefs = new List<SwarmReference>();
                
                foreach (var reference in chunkReferences)
                {
                    // Set hash as visited.
                    visitedReferences.Add(reference);
                    
                    // Get content addressed chunk and invoke callbacks.
                    SwarmChunk chunk;
                    try
                    {
                        chunk = await chunkStore.GetAsync(reference.Hash).ConfigureAwait(false);
                    }
                    catch (KeyNotFoundException)
                    {
                        await onChunkNotFoundAsync(reference.Hash).ConfigureAwait(false);
                        continue;
                    }
                    
                    if (chunk is not SwarmCac cac)
                    {
                        await onInvalidChunkFoundAsync(chunk).ConfigureAwait(false);
                        continue;
                    }
                    await onChunkFoundAsync(cac).ConfigureAwait(false);
                    
                    // Decrypt chunk.
                    ReadOnlyMemory<byte> cacData;
                    if (reference.IsEncrypted)
                    {
                        var dataLength = ChunkEncrypter.DecryptChunk(
                            cac, reference.EncryptionKey!.Value, spanBuffer, dataBuffer, hasher);
                        cacData = dataBuffer.AsMemory(0, dataLength);
                    }
                    else
                    {
                        cac.Span.CopyTo(spanBuffer);
                        cacData = cac.Data;
                    }

                    // Skip iteration on data chunks.
                    if (SwarmCac.DecodedSpanToLength(spanBuffer) <= SwarmCac.DataSize)
                        continue;

                    // Decode child chunk.
                    for (int i = 0; i < cacData.Length;)
                    {
                        //read reference
                        var childReference = new SwarmReference(cacData[i..(i + referenceSize)]);
                        i += referenceSize;

                        // Skip if already visited.
                        if (!visitedReferences.Contains(childReference))
                            nextLevelChunkRefs.Add(childReference);
                    }
                }
                
                // Iterate on next level chunks.
                chunkReferences = nextLevelChunkRefs;
            }
        }
        
        private async Task TraverseMantarayNodeHelperAsync(
            ReferencedMantarayNode manifestNode,
            HashSet<SwarmReference> visitedReferences,
            Func<SwarmChunk, Task> onChunkFoundAsync,
            Func<SwarmChunk, Task> onInvalidChunkFoundAsync,
            Func<SwarmHash, Task> onChunkNotFoundAsync)
        {
            visitedReferences.Add(manifestNode.Reference);
            
            // Try decode manifest.
            try
            {
                await manifestNode.OnVisitingAsync().ConfigureAwait(false);
            }
            catch (KeyNotFoundException)
            {
                await onChunkNotFoundAsync(manifestNode.Reference.Hash).ConfigureAwait(false);
                return;
            }
            await onChunkFoundAsync(manifestNode.Chunk!).ConfigureAwait(false);
            
            // Traverse forks.
            foreach (var fork in manifestNode.Forks.Values)
            {
                //skip already visited chunks
                if (visitedReferences.Contains(fork.Node.Reference))
                    continue;
                
                await TraverseMantarayNodeHelperAsync(
                    (ReferencedMantarayNode)fork.Node,
                    visitedReferences,
                    onChunkFoundAsync,
                    onInvalidChunkFoundAsync,
                    onChunkNotFoundAsync).ConfigureAwait(false);
            }
            
            // Traverse data.
            if (manifestNode.EntryReference.HasValue &&
                !SwarmReference.IsZero(manifestNode.EntryReference.Value) &&
                !visitedReferences.Contains(manifestNode.EntryReference.Value)) //skip already visited chunks
                await TraverseDataHelperAsync(
                    manifestNode.EntryReference.Value,
                    visitedReferences,
                    onChunkFoundAsync,
                    onInvalidChunkFoundAsync,
                    onChunkNotFoundAsync).ConfigureAwait(false);
        }
    }
}