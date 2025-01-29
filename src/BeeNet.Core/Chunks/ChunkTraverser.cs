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
        public async Task TraverseFromDataChunkAsync(
            SwarmChunkReference chunkReference,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            ArgumentNullException.ThrowIfNull(chunkReference, nameof(chunkReference));
            
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;
            HashSet<SwarmHash> visitedHashes = new HashSet<SwarmHash>();

            // Read as data chunk.
            await TraverseDataHelperAsync(
                chunkReference,
                visitedHashes,
                onChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }
        
        public async Task TraverseFromMantarayManifestRootAsync(
            SwarmHash rootHash,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;
            HashSet<SwarmHash> visitedHashes = new HashSet<SwarmHash>();
            
            // Read as manifest.
            var manifest = new ReferencedMantarayManifest(chunkStore, rootHash, true);
            var manifestNode = (ReferencedMantarayNode)manifest.RootNode;
            await TraverseMantarayNodeHelperAsync(
                manifestNode,
                visitedHashes,
                onChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }

        public Task TraverseFromMantarayNodeChunkAsync(
            SwarmHash nodeHash,
            XorEncryptKey? encryptKey,
            bool? useRecursiveEncryption,
            NodeType nodeTypeFlags,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            // Build metadata.
            var metadata = new Dictionary<string, string>();
            if (encryptKey is not null)
                metadata.Add(ManifestEntry.ChunkEncryptKeyKey, encryptKey.ToString());
            if (useRecursiveEncryption.HasValue)
                metadata.Add(ManifestEntry.UseRecursiveEncryptionKey, useRecursiveEncryption.Value.ToString());

            // Traverse.
            return TraverseFromMantarayNodeChunkAsync(
                nodeHash,
                metadata,
                nodeTypeFlags,
                onChunkFoundAsync,
                onChunkNotFoundAsync);
        }

        public async Task TraverseFromMantarayNodeChunkAsync(
            SwarmHash nodeHash,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags,
            Func<SwarmChunk, Task>? onChunkFoundAsync,
            Func<SwarmHash, Task>? onChunkNotFoundAsync)
        {
            onChunkFoundAsync ??= _ => Task.CompletedTask;
            onChunkNotFoundAsync ??= _ => Task.CompletedTask;
            HashSet<SwarmHash> visitedHashes = new HashSet<SwarmHash>();

            // Read as manifest node.
            var manifestNode = new ReferencedMantarayNode(
                chunkStore,
                nodeHash,
                metadata,
                nodeTypeFlags,
                true);
            await TraverseMantarayNodeHelperAsync(
                manifestNode,
                visitedHashes,
                onChunkFoundAsync,
                onChunkNotFoundAsync).ConfigureAwait(false);
        }

        // Helpers.
        private async Task TraverseDataHelperAsync(
            SwarmChunkReference chunkReference,
            HashSet<SwarmHash> visitedHashes,
            Func<SwarmChunk, Task> onChunkFoundAsync,
            Func<SwarmHash, Task> onChunkNotFoundAsync)
        {
            visitedHashes.Add(chunkReference.Hash);
            
            // Read and decrypt chunk data.
            SwarmChunk chunk;
            try
            {
                chunk = await chunkStore.GetAsync(chunkReference.Hash).ConfigureAwait(false);
            }
#pragma warning disable CA1031
            catch
            {
                await onChunkNotFoundAsync(chunkReference.Hash).ConfigureAwait(false);
                return;
            }
#pragma warning restore CA1031
            await onChunkFoundAsync(chunk).ConfigureAwait(false);
            
            // Extract data and decrypt.
            var dataArray = chunk.Data.ToArray();
            chunkReference.EncryptionKey?.EncryptDecrypt(dataArray);
            
            // Determine if is a data chunk, or an intermediate chunk.
            var totalDataLength = SwarmChunk.SpanToLength(chunk.Span.Span);
            if (totalDataLength <= SwarmChunk.DataSize)
                return;
            
            for (int i = 0; i < dataArray.Length;)
            {
                // Decode child chunk.
                //read hash
                var childHash = new SwarmHash(dataArray[i..(i + SwarmHash.HashSize)]);
                i += SwarmHash.HashSize;
                
                //read encryption key
                XorEncryptKey? childEncryptionKey = null;
                if (chunkReference.UseRecursiveEncryption)
                {
                    childEncryptionKey = new XorEncryptKey(dataArray[i..(i + XorEncryptKey.KeySize)]);
                    i += XorEncryptKey.KeySize;
                }

                // Skip if already visited.
                if (visitedHashes.Contains(childHash))
                    continue;

                // Traverse recursively.
                await TraverseDataHelperAsync(
                    new SwarmChunkReference(
                        childHash,
                        childEncryptionKey,
                        chunkReference.UseRecursiveEncryption),
                    visitedHashes,
                    onChunkFoundAsync,
                    onChunkNotFoundAsync).ConfigureAwait(false);
            }
        }
        
        private async Task TraverseMantarayNodeHelperAsync(
            ReferencedMantarayNode manifestNode,
            HashSet<SwarmHash> visitedHashes,
            Func<SwarmChunk, Task> onChunkFoundAsync,
            Func<SwarmHash, Task> onChunkNotFoundAsync)
        {
            visitedHashes.Add(manifestNode.Hash);
            
            // Try decode manifest.
            try
            {
                await manifestNode.DecodeFromChunkAsync().ConfigureAwait(false);
            }
#pragma warning disable CA1031
            catch
            {
                await onChunkNotFoundAsync(manifestNode.Hash).ConfigureAwait(false);
                return;
            }
#pragma warning restore CA1031
            await onChunkFoundAsync(await chunkStore.GetAsync(manifestNode.Hash).ConfigureAwait(false)).ConfigureAwait(false);
            
            // Traverse forks.
            foreach (var fork in manifestNode.Forks.Values)
            {
                //skip already visited chunks
                if (visitedHashes.Contains(fork.Node.Hash))
                    continue;
                
                await TraverseMantarayNodeHelperAsync(
                    fork.Node,
                    visitedHashes,
                    onChunkFoundAsync,
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
                    onChunkNotFoundAsync).ConfigureAwait(false);
        }
    }
}