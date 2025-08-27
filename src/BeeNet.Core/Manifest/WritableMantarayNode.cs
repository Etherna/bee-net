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

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public sealed class WritableMantarayNode : MantarayNodeBase
    {
        // Fields.
        private SwarmReference? _entryReference;
        private readonly Dictionary<char, MantarayNodeFork> _forks = new();
        private IReadOnlyDictionary<string, string> _metadata = new Dictionary<string, string>();
        private NodeType _nodeTypeFlags;
        private EncryptionKey256? _obfuscationKey;
        private SwarmReference? _reference;
        private bool skipWriteEntryHash = true;

        // Constructors.
        public WritableMantarayNode(
            ushort obfuscationCompactLevel = 0,
            EncryptionKey256? obfuscationKey = null)
        {
            if (obfuscationCompactLevel > 0 && obfuscationKey != null)
                throw new ArgumentException("Can't preset obfuscation key with compactLevel > 0");
            
            ObfuscationCompactLevel = obfuscationCompactLevel;
            _obfuscationKey = obfuscationKey;
        }

        // Properties.
        public override SwarmReference? EntryReference => _entryReference;
        public override IReadOnlyDictionary<char, MantarayNodeFork> Forks => _forks;
        public override SwarmReference Reference => _reference ?? throw new InvalidOperationException("Hash not computed");
        public override IReadOnlyDictionary<string, string> Metadata => _metadata;
        public override NodeType NodeTypeFlags => _nodeTypeFlags;
        public ushort ObfuscationCompactLevel { get; }
        public override EncryptionKey256? ObfuscationKey => _obfuscationKey;

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));
            if (path.Any(c => c >= byte.MaxValue))
                throw new ArgumentException("path only support ASCII chars", nameof(path));

            if (_reference is not null)
                throw new InvalidOperationException("Hash already calculated, the node is immutable now");

            // Determine if the last entry is not a directory. In that case, force writing entry hash.
            if (!entry.IsDirectory)
                skipWriteEntryHash = false;

            // If the new entry doesn't have a path, this becomes a value node and directly takes the entry.
            if (path.Length == 0)
            {
                SetNodeTypeFlag(NodeType.Value);
                
                _entryReference = entry.Reference;
                if (entry.Metadata.Count > 0)
                {
                    _metadata = entry.Metadata;
                    SetNodeTypeFlag(NodeType.WithMetadata);
                }
            }

            // Else, set this as an edge node, and pass entry to a fork node.
            else
            {
                // If already exists a fork that contains the path.
                if (_forks.TryGetValue(path[0], out var fork))
                {
                    var commonPrefix = fork.Prefix.FindCommonPrefix(path);
                    
                    // If the fork prefix doesn't contain the path, split it in parent and child. 
                    if (fork.Prefix.Length > commonPrefix.Length)
                    {
                        var childPrefix = fork.Prefix[commonPrefix.Length..];
                        var childNode = (WritableMantarayNode)fork.Node;
                        childNode.UpdateFlagIsWithPathSeparator(childPrefix);
                        
                        // Create new parent node.
                        //parentPrefix = commonPrefix
                        var parentNode = new WritableMantarayNode(ObfuscationCompactLevel, ObfuscationKey)
                        {
                            _forks = { [childPrefix[0]] = new MantarayNodeFork(childPrefix, childNode) },
                            skipWriteEntryHash = skipWriteEntryHash
                        };

                        //if parent node has same prefix of path, parent node is value type
                        if (commonPrefix.Length == path.Length)
                            parentNode.SetNodeTypeFlag(NodeType.Value);
                        parentNode.SetNodeTypeFlag(NodeType.Edge);

                        parentNode.UpdateFlagIsWithPathSeparator(path);
                        parentNode.Add(path[commonPrefix.Length..], entry);
                        
                        // Replace fork with the new one.
                        _forks[path[0]] = new MantarayNodeFork(commonPrefix, parentNode);
                    }
                    else // Else, reuse the same fork node.
                    {
                        ((WritableMantarayNode)fork.Node).UpdateFlagIsWithPathSeparator(path);
                        ((WritableMantarayNode)fork.Node).Add(path[commonPrefix.Length..], entry);
                    }
                }

                // Else, create the new fork for the path.
                else
                {
                    // Check for prefix size limit.
                    var prefix = path.Length > MantarayNodeFork.PrefixMaxSize ?
                        path[..MantarayNodeFork.PrefixMaxSize] : path;
                    var prefixRest = path.Length > MantarayNodeFork.PrefixMaxSize ?
                        path[MantarayNodeFork.PrefixMaxSize..] : "";
                    
                    var newNode = new WritableMantarayNode(ObfuscationCompactLevel, ObfuscationKey)
                    {
                        skipWriteEntryHash = skipWriteEntryHash
                    };

                    newNode.Add(prefixRest, entry);
                    newNode.UpdateFlagIsWithPathSeparator(prefix);

                    _forks[path[0]] = new MantarayNodeFork(prefix, newNode);
                }

                SetNodeTypeFlag(NodeType.Edge);
            }
        }

        public async Task ComputeHashAsync(
            Hasher hasher,
            BuildHasherPipeline hasherPipelineBuilder)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(hasherPipelineBuilder, nameof(hasherPipelineBuilder));
            
            if (_reference != null)
                return;

            // Recursively compute hash for each fork nodes.
            foreach (var fork in _forks.Values)
                await ((WritableMantarayNode)fork.Node).ComputeHashAsync(hasher, hasherPipelineBuilder).ConfigureAwait(false);

            // Marshal current node, and set its hash.
            if (ObfuscationCompactLevel == 0)
                _obfuscationKey ??= EncryptionKey256.BuildNewRandom(); //set random obfuscation key if missing
            else
                _obfuscationKey = await GetBestObfuscationKeyAsync(hasher, hasherPipelineBuilder).ConfigureAwait(false);
            
            var byteArray = ToByteArray(_obfuscationKey.Value);
            using var hasherPipeline = hasherPipelineBuilder(false);
            _reference = await hasherPipeline.HashDataAsync(byteArray).ConfigureAwait(false);
            
            // Clean forks.
            _forks.Clear();
        }
        
        public override Task OnVisitingAsync() => Task.CompletedTask;

        // Helpers.
        private byte[] ForksToByteArray()
        {
            // Create a fork index of 32 bytes size, using each bit to represent the existence of the key.
            // Keys are ASCII chars in [0, 255], and can't be duplicated. We can map presence in a space of 32*8 bits.
            // After the index, write the serialized forks bytes.
            
            List<byte> bytes = [];
            
            //index
            var index = new byte[ForksIndexSize];
            foreach (var k in _forks.Keys)
                index[(byte)k / 8] |= (byte)(1 << (k % 8));
            
            bytes.AddRange(index);

            //forks
            foreach (var fork in _forks.OrderBy(f => f.Key))
                bytes.AddRange(fork.Value.ToByteArray());

            return bytes.ToArray();
        }
        
        private async Task<EncryptionKey256> GetBestObfuscationKeyAsync(
            Hasher hasher,
            BuildHasherPipeline hasherPipelineBuilder)
        {
            /*
             * Calculate an obfuscation key, and try to find a bucket with optimal collisions.
             *
             * The chunk key is calculated from the plain chunk hash, replacing the last 2 bytes
             * with the attempt counter (ushort), and then hashing again.
             *
             *     chunkKey = Keccack(plainChunkHash[^2..] + attempt)
             *
             * Optimized chunk is calculated encrypting data with the obfuscation key.
             *
             * The algorithm will search the first best chunk available, trying a max of
             * incremental attempts with max at the "compactionLevel" parameter.
             *
             * Best chunk is a chunk that fits in a bucket with the lowest possible number of collisions.
             *
             * In case that a chunk hash has already been stored into stamp store, accept it without optimistic check.
             * This is the best case of all, because it will not increment any bucket.
             */

            // Calculate plain hash, and use as starting seed.
            var plainByteArray = ToByteArray(EncryptionKey256.Zero);
            using var plainHasherPipeline = hasherPipelineBuilder(true);
            var plainHashingResult = await plainHasherPipeline.HashDataAsync(plainByteArray).ConfigureAwait(false);
            var plainChunkHashArray = plainHashingResult.Hash.ToByteArray();

            // Search best chunk key.
            uint bestCollisions = 0;
            var bestKey = EncryptionKey256.Zero;

            for (ushort i = 0; i < ObfuscationCompactLevel; i++)
            {
                // Create key and data byte array.
                BinaryPrimitives.WriteUInt16BigEndian(plainChunkHashArray.AsSpan()[^2..], i);
                var obfuscationKey = new EncryptionKey256(hasher.ComputeHash(plainChunkHashArray));
                var obfuscatedData = ToByteArray(obfuscationKey);
                
                // Calculate hash and count collisions.
                using var hasherPipeline = hasherPipelineBuilder(true);
                var hashingResult = await hasherPipeline.HashDataAsync(obfuscatedData).ConfigureAwait(false);
                var collisions = hasherPipeline.PostageStamper.StampIssuer.Buckets.GetCollisions(hashingResult.Hash.ToBucketId());
                
                // Check if hash was already stamped.
                if (hasherPipeline.PostageStamper.StampStore.TryGet(
                        StampStoreItem.BuildId(hasherPipeline.PostageStamper.StampIssuer.PostageBatch.Id, hashingResult.Hash),
                        out _))
                    return obfuscationKey;
                
                // If collisions are optimal, chose this.
                if (collisions == hasherPipeline.PostageStamper.StampIssuer.Buckets.MinBucketCollisions)
                    return obfuscationKey;
                
                // If is the first attempt, or a better one.
                if (i == 0 || collisions < bestCollisions)
                {
                    bestCollisions = collisions;
                    bestKey = obfuscationKey;
                }
            }

            return bestKey;
        }

        private void RemoveNodeTypeFlag(NodeType flag) =>
            _nodeTypeFlags &= ~flag;

        private void SetNodeTypeFlag(NodeType flag) =>
            _nodeTypeFlags |= flag;
        
        private byte[] ToByteArray(EncryptionKey256 obfuscationKey)
        {
            var bytes = new List<byte>();
            
            // Write obfuscation key.
            bytes.AddRange(obfuscationKey.ToByteArray());
            
            // Write version.
            bytes.AddRange(Version02Hash);

            // Write last entry hash.
            bytes.Add((byte)(skipWriteEntryHash ? 0 : SwarmHash.HashSize));
            if (!skipWriteEntryHash)
                bytes.AddRange((EntryReference ?? SwarmReference.Zero).ToByteArray());

            // Write forks.
            bytes.AddRange(ForksToByteArray());

            // Obfuscate with key (except for key as first value).
            var bytesArray = bytes.ToArray();
            obfuscationKey.XorEncryptDecrypt(bytesArray.AsSpan()[EncryptionKey256.KeySize..]);
            return bytesArray;
        }

        private void UpdateFlagIsWithPathSeparator(string path)
        {
            if (path.IndexOf(SwarmAddress.Separator, StringComparison.InvariantCulture) > 0)
                SetNodeTypeFlag(NodeType.WithPathSeparator);
            else
                RemoveNodeTypeFlag(NodeType.WithPathSeparator);
        }
    }
}