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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using Newtonsoft.Json;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public sealed class ReferencedMantarayNode : MantarayNodeBase
    {
        // Fields.
        private SwarmReference? _entryReference;
        private readonly Dictionary<char, MantarayNodeFork> _forks = new();
        private readonly Dictionary<string, string> _metadata;
        private EncryptionKey256? _obfuscationKey;
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly RedundancyStrategy redundancyStrategy;
        private readonly bool redundancyStrategyFallback;

        // Constructor.
        public ReferencedMantarayNode(
            SwarmCac chunk,
            SwarmReference reference,
            IReadOnlyChunkStore chunkStore,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags)
        {
            ArgumentNullException.ThrowIfNull(chunk);
            
            if (chunk.Hash != reference.Hash)
                throw new ArgumentException($"Chunk's hash {chunk.Hash} does not match reference {reference}");

            this.chunkStore = chunkStore;
            this.redundancyStrategy = redundancyStrategy;
            this.redundancyStrategyFallback = redundancyStrategyFallback;
            _metadata = metadata ?? new Dictionary<string, string>();
            Chunk = chunk;
            NodeTypeFlags = nodeTypeFlags;
            Reference = reference;
        }

        // Static builders.
        public static async Task<ReferencedMantarayNode> BuildNewAsync(
            SwarmReference reference,
            IReadOnlyChunkStore chunkStore,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunkStore);
            
            // Use chunk redundancy resolver if required.
            var rootChunkStore = redundancyLevel == RedundancyLevel.None ?
                chunkStore :
                new ReplicaResolverChunkStore(chunkStore, redundancyLevel, new Hasher());
            
            // Resolve root chunk.
            var rootChunk = await rootChunkStore.GetAsync(
                reference.Hash,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            if (rootChunk is not SwarmCac rootCac) //soc are not supported
                throw new InvalidOperationException($"Chunk {reference} is not a Content Addressed Chunk.");
            
            return new ReferencedMantarayNode(
                rootCac,
                reference,
                chunkStore,
                redundancyStrategy,
                redundancyStrategyFallback,
                metadata,
                nodeTypeFlags);
        }

        // Properties.
        public SwarmCac Chunk { get; }
        public override SwarmReference? EntryReference => IsDecoded
            ? _entryReference
            : throw new InvalidOperationException("Node is not decoded from chunk");
        public override IReadOnlyDictionary<char, MantarayNodeFork> Forks => IsDecoded
            ? _forks
            : throw new InvalidOperationException("Node is not decoded from chunk");
        public bool IsDecoded { get; private set; }
        public override IReadOnlyDictionary<string, string> Metadata => _metadata;
        public override NodeType NodeTypeFlags { get; }
        public override EncryptionKey256? ObfuscationKey => IsDecoded
            ? _obfuscationKey
            : throw new InvalidOperationException("Node is not decoded from chunk");
        public override SwarmReference Reference { get; }

        // Methods.
        public async Task DecodeFromChunkAsync(
            RedundancyLevel redundancyLevel,
            CancellationToken cancellationToken = default)
        {
            if (IsDecoded)
                return;
            
            // Get chunk data.
            var decodedChunk = (SwarmDecodedDataCac)Chunk.Decode(Reference, new Hasher());
            var data = decodedChunk.Data.ToArray();

            // Get obfuscation key and de-obfuscate.
            var readIndex = 0;
            _obfuscationKey = new EncryptionKey256(data.AsMemory()[..EncryptionKey256.KeySize]);
            _obfuscationKey.Value.XorEncryptDecrypt(data.AsSpan()[EncryptionKey256.KeySize..]);
            readIndex += EncryptionKey256.KeySize;
            
            // Read header.
            var versionHash = data.AsMemory()[readIndex..(readIndex + VersionHashSize)];
            readIndex += VersionHashSize;
            
            // Decode version.
            if (versionHash.Span.SequenceEqual(Version02Hash))
                await DecodeVersion02Async(
                    data.AsMemory()[readIndex..],
                    redundancyLevel,
                    cancellationToken).ConfigureAwait(false);
            else
                throw new InvalidOperationException("Manifest version not recognized");

            IsDecoded = true;
        }
        
        public override async Task OnVisitingAsync(CancellationToken cancellationToken = default)
        {
            if (!IsDecoded)
                await DecodeFromChunkAsync(RedundancyLevel.Paranoid, cancellationToken).ConfigureAwait(false);
        }

        // Helpers.
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
        private async Task DecodeVersion02Async(
            ReadOnlyMemory<byte> data,
            RedundancyLevel redundancyLevel,
            CancellationToken cancellationToken)
        {
            var readIndex = 0;
            
            // Read last entry hash.
            var entryReferenceSize = data.Span[readIndex];
            readIndex++;
            
            if (entryReferenceSize != 0)
            {
                _entryReference = new SwarmReference(data[readIndex..(readIndex + entryReferenceSize)]);
                readIndex += entryReferenceSize;
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

                //read child node reference
                var childNodeReference = new SwarmReference(data[readIndex..(readIndex + entryReferenceSize)]);
                readIndex += entryReferenceSize;
                
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
                var forkNode = await BuildNewAsync(
                    childNodeReference,
                    chunkStore,
                    redundancyLevel,
                    redundancyStrategy,
                    redundancyStrategyFallback,
                    childNodeMetadata,
                    childNodeTypeFlags,
                    cancellationToken).ConfigureAwait(false);
                _forks[key] = new MantarayNodeFork(prefix, forkNode);
            }
        }
    }
}