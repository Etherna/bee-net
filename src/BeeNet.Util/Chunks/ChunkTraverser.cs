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
        public async Task<long> TraverseDataAsync(
            SwarmChunkReference chunkReference,
            Action<SwarmChunk>? onChunkFound,
            Action<SwarmHash>? onChunkNotFound)
        {
            ArgumentNullException.ThrowIfNull(chunkReference, nameof(chunkReference));
            
            onChunkFound ??= _ => { };
            onChunkNotFound ??= _ => { };
            HashSet<SwarmHash> visitedHashes = new HashSet<SwarmHash>();

            // Read as data chunk.
            await TraverseDataHelperAsync(
                chunkReference,
                visitedHashes,
                onChunkFound,
                onChunkNotFound).ConfigureAwait(false);
            return visitedHashes.Count;
        }
        
        public async Task<long> TraverseMantarayManifestAsync(
            SwarmHash rootHash,
            Action<SwarmChunk>? onChunkFound,
            Action<SwarmHash>? onChunkNotFound)
        {
            onChunkFound ??= _ => { };
            onChunkNotFound ??= _ => { };
            HashSet<SwarmHash> visitedHashes = new HashSet<SwarmHash>();
            
            // Read as manifest.
            var manifest = new ReferencedMantarayManifest(chunkStore, rootHash, true);
            var manifestNode = (ReferencedMantarayNode)manifest.RootNode;
            await TraverseMantarayNodeHelperAsync(
                manifestNode,
                visitedHashes,
                onChunkFound,
                onChunkNotFound).ConfigureAwait(false);
            return visitedHashes.Count;
        }

        public Task<long> TraverseMantarayNodeAsync(
            SwarmHash nodeHash,
            XorEncryptKey? encryptKey,
            bool? useRecursiveEncryption,
            NodeType nodeTypeFlags,
            Action<SwarmChunk>? onChunkFound,
            Action<SwarmHash>? onChunkNotFound)
        {
            // Build metadata.
            var metadata = new Dictionary<string, string>();
            if (encryptKey is not null)
                metadata.Add(ManifestEntry.ChunkEncryptKeyKey, encryptKey.ToString());
            if (useRecursiveEncryption.HasValue)
                metadata.Add(ManifestEntry.UseRecursiveEncryptionKey, useRecursiveEncryption.Value.ToString());

            // Traverse.
            return TraverseMantarayNodeAsync(
                nodeHash,
                metadata,
                nodeTypeFlags,
                onChunkFound,
                onChunkNotFound);
        }

        public async Task<long> TraverseMantarayNodeAsync(
            SwarmHash nodeHash,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags,
            Action<SwarmChunk>? onChunkFound,
            Action<SwarmHash>? onChunkNotFound)
        {
            onChunkFound ??= _ => { };
            onChunkNotFound ??= _ => { };
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
                onChunkFound,
                onChunkNotFound).ConfigureAwait(false);
            return visitedHashes.Count;
        }

        // Helpers.
        private async Task TraverseDataHelperAsync(
            SwarmChunkReference chunkReference,
            HashSet<SwarmHash> visitedHashes,
            Action<SwarmChunk> onChunkFound,
            Action<SwarmHash> onChunkNotFound)
        {
            // Read and decrypt chunk data.
            SwarmChunk chunk;
            try
            {
                chunk = await chunkStore.GetAsync(chunkReference.Hash).ConfigureAwait(false);
            }
#pragma warning disable CA1031
            catch
            {
                onChunkNotFound(chunkReference.Hash);
                return;
            }
#pragma warning restore CA1031
            visitedHashes.Add(chunkReference.Hash);
            onChunkFound(chunk);
            
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
                    onChunkFound,
                    onChunkNotFound).ConfigureAwait(false);
            }
        }
        
        private async Task TraverseMantarayNodeHelperAsync(
            ReferencedMantarayNode manifestNode,
            HashSet<SwarmHash> visitedHashes,
            Action<SwarmChunk> onChunkFound,
            Action<SwarmHash> onChunkNotFound)
        {
            // Try decode manifest.
            try
            {
                await manifestNode.DecodeFromChunkAsync().ConfigureAwait(false);
            }
#pragma warning disable CA1031
            catch
            {
                onChunkNotFound(manifestNode.Hash);
                return;
            }
#pragma warning restore CA1031
            visitedHashes.Add(manifestNode.Hash);
            onChunkFound(await chunkStore.GetAsync(manifestNode.Hash).ConfigureAwait(false));
            
            // Traverse forks.
            foreach (var fork in manifestNode.Forks.Values)
            {
                //skip already visited chunks
                if (visitedHashes.Contains(fork.Node.Hash))
                    continue;
                
                await TraverseMantarayNodeHelperAsync(
                    fork.Node,
                    visitedHashes,
                    onChunkFound,
                    onChunkNotFound).ConfigureAwait(false);
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
                    onChunkFound,
                    onChunkNotFound).ConfigureAwait(false);
        }
    }
}