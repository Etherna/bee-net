using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
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
        private readonly IChunkStore chunkStore;
        private SwarmHash? _entryHash;
        private readonly Dictionary<char, ReferencedMantarayNodeFork> _forks = new();
        private readonly Dictionary<string, string> _metadata;
        private XorEncryptKey? _obfuscationKey;

        // Constructor.
        public ReferencedMantarayNode(
            IChunkStore chunkStore,
            SwarmHash chunkHash,
            Dictionary<string, string>? metadata,
            NodeType nodeTypeFlags)
        {
            this.chunkStore = chunkStore ?? throw new ArgumentNullException(nameof(chunkStore));
            Hash = chunkHash;
            _metadata = metadata ?? new Dictionary<string, string>();
            NodeTypeFlags = nodeTypeFlags;
        }
        
        // Properties.
        public SwarmHash? EntryHash => IsDecoded
            ? _entryHash
            : throw new InvalidOperationException("Node is not decoded from chunk");
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

            var chunk = await chunkStore.GetAsync(Hash).ConfigureAwait(false);
            
            var data = chunk.Data.ToArray();
            var readIndex = 0;
            
            // Get obfuscation key and de-obfuscate.
            _obfuscationKey = new XorEncryptKey(data[..XorEncryptKey.KeySize]);
            _obfuscationKey.EncryptDecrypt(data.AsSpan()[XorEncryptKey.KeySize..]);
            readIndex += XorEncryptKey.KeySize;
            
            // Read header.
            var versionHash = data.AsMemory()[readIndex..(readIndex + MantarayNode.VersionHashSize)];
            readIndex += MantarayNode.VersionHashSize;
            
            if (versionHash.Span.SequenceEqual(MantarayNode.Version02Hash))
                DecodeVersion02(data.AsSpan()[readIndex..]);
            else
                throw new InvalidOperationException("Manifest version not recognized");
            
            // Set as decoded.
            IsDecoded = true;
        }

        public async Task<SwarmHash> ResolveResourceHashAsync(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            // If the path is empty
            if (path.Length == 0)
            {
                //if entry is not null, return it
                if (EntryHash.HasValue && EntryHash != SwarmHash.Zero)
                    return EntryHash.Value;
                
                //try to lookup for index document suffix
                if (!_forks.TryGetValue('/', out var rootFork) ||
                    rootFork.Prefix != "/")
                    throw new KeyNotFoundException($"Final path {path} can't be found");
                
                if (!rootFork.Node.Metadata.TryGetValue(ManifestEntry.WebsiteIndexDocPathKey, out var suffix))
                    throw new KeyNotFoundException($"Index document can't be found");

                path += suffix;
            }
            
            // Else, find the child fork.
            if (!_forks.TryGetValue(path[0], out var fork) ||
                !path.StartsWith(fork.Prefix, StringComparison.InvariantCulture))
                throw new KeyNotFoundException($"Final path {path} can't be found");

            if (!fork.Node.IsDecoded)
                await fork.Node.DecodeFromChunkAsync().ConfigureAwait(false);

            return await fork.Node.ResolveResourceHashAsync(path[fork.Prefix.Length..]).ConfigureAwait(false);
        }
        
        // Helpers.
        private void DecodeVersion02(ReadOnlySpan<byte> data)
        {
            var readIndex = 0;
            
            // Read last entry hash.
            var entryHashSize = data[readIndex];
            readIndex++;
            
            if (entryHashSize != 0)
            {
                _entryHash = new SwarmHash(data[readIndex..(readIndex + entryHashSize)].ToArray());
                readIndex += entryHashSize;
            }
            
            // Read forks.
            //index
            var forksIndex = data[readIndex..(readIndex + MantarayNode.ForksIndexSize)];
            readIndex += MantarayNode.ForksIndexSize;
            
            var forksKeys = new List<char>();
            for (int i = 0; i < MantarayNode.ForksIndexSize * 8; i++)
            {
                if ((forksIndex[i / 8] & (byte)(1 << (i % 8))) != 0)
                    forksKeys.Add((char)i);
            }
            
            //forks
            foreach (var key in forksKeys)
            {
                var childNodeTypeFlags = (NodeType)data[readIndex++];
                var prefixLength = data[readIndex++];
                
                //read prefix
                var prefix = Encoding.UTF8.GetString(data[readIndex..(readIndex + MantarayNodeFork.PrefixMaxSize)])[..prefixLength];
                readIndex += MantarayNodeFork.PrefixMaxSize;

                //read child node hash
                var childNodeHash = new SwarmHash(data[readIndex..(readIndex + SwarmHash.HashSize)].ToArray());
                readIndex += SwarmHash.HashSize;
                
                //read metadata
                Dictionary<string, string>? childNodeMetadata = null;
                if (childNodeTypeFlags.HasFlag(NodeType.WithMetadata))
                {
                    var metadataBytesLength = BinaryPrimitives.ReadUInt16BigEndian(
                        data[readIndex..(readIndex + MantarayNodeFork.MetadataBytesSize)]);
                    readIndex += MantarayNodeFork.MetadataBytesSize;

                    var metadataBytes = data[readIndex..(readIndex + metadataBytesLength)];
                    readIndex += metadataBytesLength;

                    childNodeMetadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                        Encoding.UTF8.GetString(metadataBytes));
                    
                    //skip padding
                    var metadataTotalSize = metadataBytes.Length + MantarayNodeFork.MetadataBytesSize;
                    if (metadataTotalSize % XorEncryptKey.KeySize != 0)
                        readIndex += XorEncryptKey.KeySize - metadataTotalSize % XorEncryptKey.KeySize;
                }
                
                //add fork
                _forks[key] = new ReferencedMantarayNodeFork(
                    prefix,
                    new ReferencedMantarayNode(chunkStore, childNodeHash, childNodeMetadata, childNodeTypeFlags));
            }
        }
    }
}