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

using Etherna.BeeNet.Chunks;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using Newtonsoft.Json;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public sealed class ReferencedMantarayNode : MantarayNodeBase
    {
        // Fields.
        private readonly ChunkDataStream chunkDataStream;
        private bool disposed;
        
        private SwarmReference? _entryReference;
        private readonly Dictionary<char, MantarayNodeFork> _forks = new();
        private readonly Dictionary<string, string> _metadata;
        private EncryptionKey256? _obfuscationKey;

        // Constructor.
        private ReferencedMantarayNode(
            ChunkDataStream chunkDataStream,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags)
        {
            this.chunkDataStream = chunkDataStream;
            _metadata = metadata ?? new Dictionary<string, string>();
            NodeTypeFlags = nodeTypeFlags;
        }
        
        // Dispose.
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;

            base.Dispose(disposing);
            if (disposing)
            {
                chunkDataStream.Dispose();
                foreach (var fork in _forks.Values)
                    fork.Dispose();
            }

            disposed = true;
        }
        protected override async ValueTask DisposeAsyncCore()
        {
            if (disposed) return;

            await base.DisposeAsyncCore().ConfigureAwait(false);
            await chunkDataStream.DisposeAsync().ConfigureAwait(false);
            foreach (var fork in _forks.Values)
                await fork.DisposeAsync().ConfigureAwait(false);

            disposed = true;
        }

        // Static builders.
        public static async Task<ReferencedMantarayNode> BuildNewAsync(
            SwarmReference reference,
            IReadOnlyChunkStore chunkStore,
            RedundancyLevel redundancyLevel,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags)
        {
            var chunkDataStream = await ChunkDataStream.BuildNewAsync(
                reference,
                chunkStore,
                redundancyLevel,
                redundancyStrategy,
                redundancyStrategyFallback).ConfigureAwait(false);
            
            return new ReferencedMantarayNode(
                chunkDataStream,
                metadata,
                nodeTypeFlags);
        }

        public static ReferencedMantarayNode BuildNew(
            SwarmCac chunk,
            SwarmReference reference,
            IReadOnlyChunkStore chunkStore,
            RedundancyStrategy redundancyStrategy,
            bool redundancyStrategyFallback,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags)
        {
            var chunkDataStream = ChunkDataStream.BuildNew(
                chunk,
                reference,
                chunkStore,
                redundancyStrategy,
                redundancyStrategyFallback);
            
            return new ReferencedMantarayNode(
                chunkDataStream,
                metadata,
                nodeTypeFlags);
        }

        // Properties.
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
        public override SwarmReference Reference => chunkDataStream.Reference;
        
        // Methods.
        public async Task DecodeFromChunkAsync(
            RedundancyLevel redundancyLevel)
        {
            if (IsDecoded)
                return;
            
            // Get chunk data.
            var data = await chunkDataStream.ToByteArrayAsync().ConfigureAwait(false);

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
                    redundancyLevel).ConfigureAwait(false);
            else
                throw new InvalidOperationException("Manifest version not recognized");

            IsDecoded = true;
        }
        
        public override async Task OnVisitingAsync()
        {
            if (!IsDecoded)
                await DecodeFromChunkAsync(RedundancyLevel.Paranoid).ConfigureAwait(false);
        }

        // Helpers.
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
        private async Task DecodeVersion02Async(
            ReadOnlyMemory<byte> data,
            RedundancyLevel redundancyLevel)
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
                    chunkDataStream.ChunkStore,
                    redundancyLevel,
                    chunkDataStream.RedundancyStrategy,
                    chunkDataStream.RedundancyStrategyFallback,
                    childNodeMetadata,
                    childNodeTypeFlags).ConfigureAwait(false);
                _forks[key] = new MantarayNodeFork(prefix, forkNode);
            }
        }
    }
}