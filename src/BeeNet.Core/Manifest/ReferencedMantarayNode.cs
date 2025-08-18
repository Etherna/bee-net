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
    public sealed class ReferencedMantarayNode : MantarayNodeBase
    {
        // Fields.
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly bool useChunkStoreCache;
        
        private SwarmHash? _entryHash;
        private readonly Dictionary<char, MantarayNodeFork> _forks = new();
        private readonly Dictionary<string, string> _metadata;
        private EncryptionKey256? _obfuscationKey;

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
        }
        
        public ReferencedMantarayNode(
            IReadOnlyChunkStore chunkStore,
            SwarmCac chunk,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags,
            bool useChunkStoreCache = false)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            
            this.chunkStore = chunkStore ?? throw new ArgumentNullException(nameof(chunkStore));
            this.useChunkStoreCache = useChunkStoreCache;
            Hash = chunk.Hash;
            _metadata = metadata ?? new Dictionary<string, string>();
            NodeTypeFlags = nodeTypeFlags;
            
            // Decode chunk.
            DecodeCacHelper(chunk);
        }

        // Properties.
        public SwarmCac? Chunk { get; private set; }
        public override SwarmHash? EntryHash => IsDecoded
            ? _entryHash
            : throw new InvalidOperationException("Node is not decoded from chunk");
        public override IReadOnlyDictionary<char, MantarayNodeFork> Forks => IsDecoded
            ? _forks
            : throw new InvalidOperationException("Node is not decoded from chunk");
        public override SwarmHash Hash { get; }
        public bool IsDecoded => Chunk != null;
        public override IReadOnlyDictionary<string, string> Metadata => _metadata;
        public override NodeType NodeTypeFlags { get; }
        public override EncryptionKey256? ObfuscationKey => IsDecoded
            ? _obfuscationKey
            : throw new InvalidOperationException("Node is not decoded from chunk");

        // Methods.
        public async Task DecodeFromChunkAsync()
        {
            if (IsDecoded)
                return;

            var chunk = await chunkStore.GetAsync(
                Hash,
                useChunkStoreCache).ConfigureAwait(false);
            if (chunk is not SwarmCac cac)
                throw new InvalidOperationException("Chunk is not a Content Addressed Chunk");
            
            DecodeCacHelper(cac);
        }

        public override async Task OnVisitingAsync()
        {
            if (!IsDecoded)
                await DecodeFromChunkAsync().ConfigureAwait(false);
        }
        
        // Helpers.
        private void DecodeCacHelper(SwarmCac chunk)
        {
            if (chunk.Hash != Hash)
                throw new ArgumentException("Chunk hash not match");
            
            var data = chunk.Data.ToArray();
            var readIndex = 0;
            
            // Get obfuscation key and de-obfuscate.
            _obfuscationKey = new EncryptionKey256(data.AsMemory()[..EncryptionKey256.KeySize]);
            _obfuscationKey.Value.XorEncryptDecrypt(data.AsSpan()[EncryptionKey256.KeySize..]);
            readIndex += EncryptionKey256.KeySize;
            
            // Read header.
            var versionHash = data.AsMemory()[readIndex..(readIndex + VersionHashSize)];
            readIndex += VersionHashSize;
            
            if (versionHash.Span.SequenceEqual(Version02Hash))
                DecodeVersion02(data.AsMemory()[readIndex..]);
            else
                throw new InvalidOperationException("Manifest version not recognized");
            
            // Set chunk.
            Chunk = chunk;
        }
        
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
            var forksIndex = data[readIndex..(readIndex + ForksIndexSize)];
            readIndex += ForksIndexSize;
            
            var forksKeys = new List<char>();
            for (int i = 0; i < ForksIndexSize * 8; i++)
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
                    if (metadataTotalSize % EncryptionKey256.KeySize != 0)
                        readIndex += EncryptionKey256.KeySize - metadataTotalSize % EncryptionKey256.KeySize;
                }
                
                //add fork
                _forks[key] = new MantarayNodeFork(
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