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
using Newtonsoft.Json;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class ReferencedMantarayNode : IReadOnlyMantarayNode
    {
        // Fields.
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly bool useChunkStoreCache;
        
        private SwarmHash? _entryHash;
        private readonly Dictionary<char, ReferencedMantarayNodeFork> _forks = new();
        private readonly Dictionary<string, string> _metadata;
        private XorEncryptKey? _obfuscationKey;

        // Constructor.
        public ReferencedMantarayNode(
            IReadOnlyChunkStore chunkStore,
            SwarmHash chunkHash,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags,
            bool useChunkStoreCache = false)
        {
            this.chunkStore = chunkStore ?? throw new ArgumentNullException(nameof(chunkStore));
            this.useChunkStoreCache = useChunkStoreCache;
            Hash = chunkHash;
            _metadata = metadata ?? new Dictionary<string, string>();
            NodeTypeFlags = nodeTypeFlags;

            // Read metadata.
            if (_metadata.TryGetValue(ManifestEntry.ChunkEncryptKeyKey, out var encryptKeyStr))
                EntryEncryptionKey = new XorEncryptKey(encryptKeyStr);
            if (_metadata.TryGetValue(ManifestEntry.UseRecursiveEncryptionKey, out var useRecursiveEncrypStr))
                EntryUseRecursiveEncryption = bool.Parse(useRecursiveEncrypStr);
        }
        
        // Properties.
        public XorEncryptKey? EntryEncryptionKey { get; }
        public SwarmHash? EntryHash => IsDecoded
            ? _entryHash
            : throw new InvalidOperationException("Node is not decoded from chunk");
        public bool EntryUseRecursiveEncryption { get; }
        public IReadOnlyDictionary<char, ReferencedMantarayNodeFork> Forks => IsDecoded
            ? _forks
            : throw new InvalidOperationException("Node is not decoded from chunk");
        public SwarmHash Hash { get; }
        public bool IsDecoded { get; private set; }
        public IReadOnlyDictionary<string, string> Metadata => _metadata;
        public NodeType NodeTypeFlags { get; }
        public XorEncryptKey? ObfuscationKey => IsDecoded
            ? _obfuscationKey
            : throw new InvalidOperationException("Node is not decoded from chunk");

        // Methods.
        public async Task DecodeFromChunkAsync()
        {
            if (IsDecoded)
                return;

            var chunk = await chunkStore.GetAsync(
                Hash,
                SwarmChunkType.Cac,
                useChunkStoreCache).ConfigureAwait(false);
            if (chunk is not SwarmCac cac)
                throw new InvalidOperationException("Chunk is not a Content Addressed Chunk");
            
            var data = cac.Data.ToArray();
            var readIndex = 0;
            
            // Get obfuscation key and de-obfuscate.
            _obfuscationKey = new XorEncryptKey(data.AsMemory()[..XorEncryptKey.KeySize]);
            _obfuscationKey.Value.EncryptDecrypt(data.AsSpan()[XorEncryptKey.KeySize..]);
            readIndex += XorEncryptKey.KeySize;
            
            // Read header.
            var versionHash = data.AsMemory()[readIndex..(readIndex + MantarayNode.VersionHashSize)];
            readIndex += MantarayNode.VersionHashSize;
            
            if (versionHash.Span.SequenceEqual(MantarayNode.Version02Hash))
                DecodeVersion02(data.AsMemory()[readIndex..]);
            else
                throw new InvalidOperationException("Manifest version not recognized");
            
            // Set as decoded.
            IsDecoded = true;
        }

        public async Task<SwarmChunkReference> GetChunkReferenceAsync(string path) =>
            (await GetChunkReferenceWithMetadataAsync(path).ConfigureAwait(false)).Item1;

        public async Task<(SwarmChunkReference, IReadOnlyDictionary<string, string>)> GetChunkReferenceWithMetadataAsync(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            // If the path is empty and entry is not null, return the entry
            if (path.Length == 0)
            {
                if (EntryHash.HasValue && EntryHash != SwarmHash.Zero)
                    return (new SwarmChunkReference(
                            EntryHash.Value,
                            EntryEncryptionKey,
                            EntryUseRecursiveEncryption),
                        _metadata);
            
                throw new KeyNotFoundException("Path can't be found");
            }
            
            // Find the child fork.
            if (!_forks.TryGetValue(path[0], out var fork) ||
                !path.StartsWith(fork.Prefix, StringComparison.InvariantCulture))
                throw new KeyNotFoundException($"Final path {path} can't be found");

            if (!fork.Node.IsDecoded)
                await fork.Node.DecodeFromChunkAsync().ConfigureAwait(false);

            return await fork.Node.GetChunkReferenceWithMetadataAsync(path[fork.Prefix.Length..]).ConfigureAwait(false);
        }

        public async Task<IReadOnlyDictionary<string, string>> GetMetadataAsync(
            string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            // If the path is empty, return current node metadata
            if (path.Length == 0)
                return _metadata;

            // Find the child fork.
            if (!_forks.TryGetValue(path[0], out var fork) ||
                !path.StartsWith(fork.Prefix, StringComparison.InvariantCulture))
                throw new KeyNotFoundException($"Final path {path} can't be found");
            
            // If the child node is the one we are looking for, return metadata.
            var childSubPath = path[fork.Prefix.Length..];
            if (childSubPath.Length == 0)
                return fork.Node._metadata;
            
            // Else, proceed into it.
            if (!fork.Node.IsDecoded)
                await fork.Node.DecodeFromChunkAsync().ConfigureAwait(false);

            return await fork.Node.GetMetadataAsync(childSubPath).ConfigureAwait(false);
        }

        public async Task<bool> HasPathPrefixAsync(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            if (path.Length == 0)
                return true;
            
            // Find the child fork.
            if (!_forks.TryGetValue(path[0], out var fork))
                return false;
            
            var commonPathLength = Math.Min(path.Length, fork.Prefix.Length);
            if (!path.AsSpan()[..commonPathLength].SequenceEqual(fork.Prefix.AsSpan()[..commonPathLength]))
                return false;
            
            if (!fork.Node.IsDecoded)
                await fork.Node.DecodeFromChunkAsync().ConfigureAwait(false);

            return await fork.Node.HasPathPrefixAsync(
                path[commonPathLength..]).ConfigureAwait(false);
        }
        
        // Helpers.
        private void DecodeVersion02(ReadOnlyMemory<byte> data)
        {
            var readIndex = 0;
            
            // Read last entry hash.
            var entryHashSize = data.Span[readIndex];
            readIndex++;
            
            if (entryHashSize != 0)
            {
                _entryHash = new SwarmHash(data[readIndex..(readIndex + entryHashSize)]);
                readIndex += entryHashSize;
            }
            
            // Read forks.
            //index
            var forksIndex = data[readIndex..(readIndex + MantarayNode.ForksIndexSize)];
            readIndex += MantarayNode.ForksIndexSize;
            
            var forksKeys = new List<char>();
            for (int i = 0; i < MantarayNode.ForksIndexSize * 8; i++)
            {
                if ((forksIndex.Span[i / 8] & (byte)(1 << (i % 8))) != 0)
                    forksKeys.Add((char)i);
            }
            
            //forks
            foreach (var key in forksKeys)
            {
                var childNodeTypeFlags = (NodeType)data.Span[readIndex++];
                var prefixLength = data.Span[readIndex++];
                
                //read prefix
                var prefix = Encoding.UTF8.GetString(data.Span[readIndex..(readIndex + MantarayNodeFork.PrefixMaxSize)])[..prefixLength];
                readIndex += MantarayNodeFork.PrefixMaxSize;

                //read child node hash
                var childNodeHash = new SwarmHash(data[readIndex..(readIndex + SwarmHash.HashSize)]);
                readIndex += SwarmHash.HashSize;
                
                //read metadata
                Dictionary<string, string>? childNodeMetadata = null;
                if (childNodeTypeFlags.HasFlag(NodeType.WithMetadata))
                {
                    var metadataBytesLength = BinaryPrimitives.ReadUInt16BigEndian(
                        data.Span[readIndex..(readIndex + MantarayNodeFork.MetadataBytesSize)]);
                    readIndex += MantarayNodeFork.MetadataBytesSize;

                    var metadataBytes = data[readIndex..(readIndex + metadataBytesLength)];
                    readIndex += metadataBytesLength;

                    childNodeMetadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        Encoding.UTF8.GetString(metadataBytes.Span));
                    
                    //skip padding
                    var metadataTotalSize = metadataBytes.Length + MantarayNodeFork.MetadataBytesSize;
                    if (metadataTotalSize % XorEncryptKey.KeySize != 0)
                        readIndex += XorEncryptKey.KeySize - metadataTotalSize % XorEncryptKey.KeySize;
                }
                
                //add fork
                _forks[key] = new ReferencedMantarayNodeFork(
                    prefix,
                    new ReferencedMantarayNode(
                        chunkStore,
                        childNodeHash,
                        childNodeMetadata,
                        childNodeTypeFlags,
                        useChunkStoreCache));
            }
        }
    }
}