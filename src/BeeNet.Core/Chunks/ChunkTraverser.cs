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
        /// <param name="rootHash">Root traversing chunk</param>
        public async Task TraverseAsync(
            SwarmChunkReference rootReference,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            ArgumentNullException.ThrowIfNull(rootReference, nameof(rootReference));
            
            // Identify if is manifest root chunk.
            var isManifestChunk = false;
            if (!rootReference.UseRecursiveEncryption) //manifest can't use recursive encryption
            {
                try
                {
                    var manifest = new ReferencedMantarayManifest(chunkStore, rootReference.Hash);
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
            }

            // Traverse with identified chunk type.
            if (isManifestChunk)
                await TraverseFromMantarayManifestRootAsync(
                    rootReference.Hash,
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
            SwarmChunkReference chunkReference,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            ArgumentNullException.ThrowIfNull(chunkReference, nameof(chunkReference));
            
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onInvalidChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;

            // Read as data or intermediate chunk.
            await TraverseDataHelperAsync(
                chunkReference,
                [],
                onChunkFoundAsync,
                onInvalidChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }
        
        public async Task TraverseFromMantarayManifestRootAsync(
            SwarmHash rootHash,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onInvalidChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;
            
            // Read as manifest.
            var manifest = new ReferencedMantarayManifest(chunkStore, rootHash);
            await TraverseMantarayNodeHelperAsync(
                (ReferencedMantarayNode)manifest.RootNode,
                [],
                onChunkFoundAsync,
                onInvalidChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }

        public Task TraverseFromMantarayNodeChunkAsync(
            SwarmHash nodeHash,
            XorEncryptKey? encryptKey,
            bool? useRecursiveEncryption,
            NodeType nodeTypeFlags,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmChunk, Task>? onInvalidChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            // Build metadata.
            var metadata = new Dictionary<string, string>();
            if (encryptKey is not null)
                metadata.Add(ManifestEntry.ChunkEncryptKeyKey, encryptKey.Value.ToString());
            if (useRecursiveEncryption.HasValue)
                metadata.Add(ManifestEntry.UseRecursiveEncryptionKey, useRecursiveEncryption.Value.ToString());

            // Traverse.
            return TraverseFromMantarayNodeChunkAsync(
                nodeHash,
                metadata,
                nodeTypeFlags,
                onChunkFoundAsync,
                onInvalidChunkFoundAsync,
                onChunkNotFoundAsync);
        }

        public async Task TraverseFromMantarayNodeChunkAsync(
            SwarmHash nodeHash,
            Dictionary<string, string>? metadata,
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
                nodeHash,
                metadata,
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
            SwarmChunkReference rootChunkRef,
            HashSet<SwarmHash> visitedHashes,
            Func<SwarmChunk, Task> onChunkFoundAsync,
            Func<SwarmChunk, Task> onInvalidChunkFoundAsync,
            Func<SwarmHash, Task> onChunkNotFoundAsync)
        {
            List<SwarmChunkReference> chunkRefs = [rootChunkRef];

            while (chunkRefs.Count > 0)
            {
                var nextLevelChunkRefs = new List<SwarmChunkReference>();
                
                foreach (var chunkRef in chunkRefs)
                {
                    // Set hash as visited.
                    visitedHashes.Add(chunkRef.Hash);
                    
                    // Get content addressed chunk and invoke callbacks.
                    SwarmChunk chunk;
                    try
                    {
                        chunk = await chunkStore.GetAsync(chunkRef.Hash).ConfigureAwait(false);
                    }
                    catch (KeyNotFoundException)
                    {
                        await onChunkNotFoundAsync(chunkRef.Hash).ConfigureAwait(false);
                        continue;
                    }
                    
                    if (chunk is not SwarmCac cac)
                    {
                        await onInvalidChunkFoundAsync(chunk).ConfigureAwait(false);
                        continue;
                    }
                    await onChunkFoundAsync(cac).ConfigureAwait(false);

                    // Skip iteration on data chunks.
                    if (cac.IsDataChunk)
                        continue;

                    // Extract intermediate chunk data and decrypt.
                    ReadOnlyMemory<byte> cacData;
                    if (chunkRef.EncryptionKey is null)
                    {
                        cacData = cac.Data;
                    }
                    else
                    {
                        var buffer = cac.Data.ToArray();
                        chunkRef.EncryptionKey?.EncryptDecrypt(buffer);
                        cacData = buffer;
                    }

                    // Decode child chunk.
                    for (int i = 0; i < cacData.Length;)
                    {
                        //read hash
                        var childHash = new SwarmHash(cacData[i..(i + SwarmHash.HashSize)]);
                        i += SwarmHash.HashSize;

                        //read encryption key
                        XorEncryptKey? childEncryptionKey = null;
                        if (chunkRef.UseRecursiveEncryption)
                        {
                            childEncryptionKey = new XorEncryptKey(cacData[i..(i + XorEncryptKey.KeySize)]);
                            i += XorEncryptKey.KeySize;
                        }

                        // Skip if already visited.
                        if (!visitedHashes.Contains(childHash))
                        {
                            nextLevelChunkRefs.Add(new SwarmChunkReference(
                                childHash,
                                childEncryptionKey,
                                chunkRef.UseRecursiveEncryption));
                        }
                    }
                }
                
                // Iterate on next level chunks.
                chunkRefs = nextLevelChunkRefs;
            }
        }
        
        private async Task TraverseMantarayNodeHelperAsync(
            ReferencedMantarayNode manifestNode,
            HashSet<SwarmHash> visitedHashes,
            Func<SwarmChunk, Task> onChunkFoundAsync,
            Func<SwarmChunk, Task> onInvalidChunkFoundAsync,
            Func<SwarmHash, Task> onChunkNotFoundAsync)
        {
            visitedHashes.Add(manifestNode.Hash);
            
            // Try decode manifest.
            try
            {
                await manifestNode.OnVisitingAsync().ConfigureAwait(false);
            }
            catch (KeyNotFoundException)
            {
                await onChunkNotFoundAsync(manifestNode.Hash).ConfigureAwait(false);
                return;
            }
            await onChunkFoundAsync(manifestNode.Chunk!).ConfigureAwait(false);
            
            // Traverse forks.
            foreach (var fork in manifestNode.Forks.Values)
            {
                //skip already visited chunks
                if (visitedHashes.Contains(fork.Node.Hash))
                    continue;
                
                await TraverseMantarayNodeHelperAsync(
                    (ReferencedMantarayNode)fork.Node,
                    visitedHashes,
                    onChunkFoundAsync,
                    onInvalidChunkFoundAsync,
                    onChunkNotFoundAsync).ConfigureAwait(false);
            }
            
            // Traverse data.
            if (manifestNode.EntryHash.HasValue &&
                manifestNode.EntryHash != SwarmHash.Zero &&
                !visitedHashes.Contains(manifestNode.EntryHash.Value)) //skip already visited chunks
                await TraverseDataHelperAsync(
                    new SwarmChunkReference(
                        manifestNode.EntryHash.Value,
                        manifestNode.EntryEncryptionKey,
                        manifestNode.EntryUseRecursiveEncryption),
                    visitedHashes,
                    onChunkFoundAsync,
                    onInvalidChunkFoundAsync,
                    onChunkNotFoundAsync).ConfigureAwait(false);
        }
    }
}