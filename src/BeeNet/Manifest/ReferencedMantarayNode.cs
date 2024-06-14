using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class ReferencedMantarayNode : IReadOnlyMantarayNode
    {
        // Fields.
        private SwarmHash? _entryHash = default!;
        private readonly IChunkStore chunkStore;
        private readonly Dictionary<string, string> _metadata = new();
        private NodeType _nodeTypeFlags= default!;
        private XorEncryptKey? _obfuscationKey= default!;

        // Constructor.
        public ReferencedMantarayNode(IChunkStore chunkStore, SwarmHash chunkHash)
        {
            Hash = chunkHash;
            this.chunkStore = chunkStore ?? throw new ArgumentNullException(nameof(chunkStore));
        }
        
        // Properties.
        public SwarmHash? EntryHash =>
            IsDecoded ? _entryHash : throw new InvalidOperationException();
        public SwarmHash Hash { get; }
        public bool IsDecoded { get; private set; }
        public IReadOnlyDictionary<string, string> Metadata =>
            IsDecoded ? _metadata : throw new InvalidOperationException();
        public NodeType NodeTypeFlags =>
            IsDecoded ? _nodeTypeFlags : throw new InvalidOperationException();
        public XorEncryptKey? ObfuscationKey =>
            IsDecoded ? _obfuscationKey : throw new InvalidOperationException();
        
        // Methods.
        public async Task<bool> TryDecodeFromChunkAsync()
        {
            if (IsDecoded)
                return true;

            var chunk = await chunkStore.GetAsync(Hash).ConfigureAwait(false);
            
            var data = chunk.Data.ToArray();
            var readIndex = 0;
            
            // Get obfuscation key and de-obfuscate.
            _obfuscationKey = new XorEncryptKey(data[..XorEncryptKey.KeySize]);
            _obfuscationKey.EncryptDecrypt(data.AsSpan()[XorEncryptKey.KeySize..]);
            readIndex += XorEncryptKey.KeySize;
            
            // Read header.
            var versionHash = data.AsMemory()[readIndex..(readIndex + MantarayNode.VersionHashSize)];
            if (!versionHash.Equals(MantarayNode.Version02Hash.AsMemory()))
                throw new InvalidOperationException("Manifest version not recognized");
            readIndex += MantarayNode.VersionHashSize;
            
            // Read last entry hash.
            var entryHashSize = data[readIndex];
            readIndex++;
            
            if (entryHashSize != 0)
            {
                _entryHash = new SwarmHash(data[readIndex..(readIndex + entryHashSize)]);
                readIndex += entryHashSize;
            }
            
            // Read forks.
            var forksIndex = data[readIndex..(readIndex + MantarayNode.ForksIndexSize)];
            //TODO

            throw new NotImplementedException();
        }
    }
}