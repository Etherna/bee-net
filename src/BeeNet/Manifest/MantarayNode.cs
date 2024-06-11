// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Epoche;
using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class MantarayNode
    {
        // Consts.
        private const int NodeHeaderSize = VersionHashSize + NodeRefBytesSize;
        private const int NodeRefBytesSize = 1;
        private const char PathSeparator = '/';
        private static readonly byte[] Version02Hash =
            Keccak256.ComputeHash("mantaray:0.2").Take(VersionHashSize).ToArray();
        private const int VersionHashSize = 31;
        
        // Fields.
        private SwarmHash? _hash;
        private readonly Dictionary<char, MantarayNodeFork> forks = new();
        private bool skipWriteEntryHash;

        public MantarayNode(XorEncryptKey? obfuscationKey)
        {
            ObfuscationKey = obfuscationKey;
            skipWriteEntryHash = true;
        }

        // Properties.
        public SwarmHash Hash => _hash ?? throw new InvalidOperationException("Hash not computed");
        public SwarmHash? EntryHash { get; private set; }
        public IReadOnlyDictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();
        public NodeType NodeTypeFlags { get; private set; }
        public XorEncryptKey? ObfuscationKey { get; private set; }

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));
            if (path.Any(c => c >= byte.MaxValue))
                throw new ArgumentException("path only support ASCII chars", nameof(path));

            if (_hash is not null)
                throw new InvalidOperationException("Hash already calculated, the node is immutable now");

            // Determine if the last entry is not a directory. In that case, force writing entry hash.
            if (!entry.IsDirectory)
                skipWriteEntryHash = false;

            // If the new entry doesn't have a path, this become a value node and directly takes entry.
            if (path.Length == 0)
            {
                SetNodeTypeFlag(NodeType.Value);
                
                EntryHash = entry.Hash;
                if (entry.Metadata.Count > 0)
                {
                    Metadata = entry.Metadata;
                    SetNodeTypeFlag(NodeType.WithMetadata);
                }
            }

            // Else, set this as an edge node, and pass entry to a fork node.
            else
            {
                // If already exists a fork that contains the path.
                if (forks.TryGetValue(path[0], out var fork))
                {
                    var commonPrefix = fork.Prefix.FindCommonPrefix(path);
                    
                    // If the fork prefix doesn't contain the path, split it in parent and child. 
                    if (fork.Prefix.Length > commonPrefix.Length)
                    {
                        var childPrefix = fork.Prefix[commonPrefix.Length..];
                        var childNode = fork.Node;
                        childNode.UpdateFlagIsWithPathSeparator(childPrefix);
                        
                        // Create new parent node.
                        //parentPrefix = commonPrefix
                        var parentNode = new MantarayNode(ObfuscationKey)
                        {
                            forks = { [childPrefix[0]] = new MantarayNodeFork(childPrefix, childNode) },
                            skipWriteEntryHash = skipWriteEntryHash
                        };

                        //if parent node has same prefix of path, parent node is value type
                        if (commonPrefix.Length == path.Length)
                            parentNode.SetNodeTypeFlag(NodeType.Value);
                        parentNode.SetNodeTypeFlag(NodeType.Edge);

                        parentNode.UpdateFlagIsWithPathSeparator(path);
                        parentNode.Add(path[commonPrefix.Length..], entry);
                        
                        // Replace fork with the new one.
                        forks[path[0]] = new MantarayNodeFork(commonPrefix, parentNode);
                    }
                    else // Else, reuse the same fork node.
                    {
                        fork.Node.UpdateFlagIsWithPathSeparator(path);
                        fork.Node.Add(path[commonPrefix.Length..], entry);
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
                    
                    var newNode = new MantarayNode(ObfuscationKey)
                    {
                        skipWriteEntryHash = skipWriteEntryHash
                    };

                    newNode.Add(prefixRest, entry);
                    newNode.UpdateFlagIsWithPathSeparator(prefix);

                    forks[path[0]] = new MantarayNodeFork(prefix, newNode);
                }

                SetNodeTypeFlag(NodeType.Edge);
            }
        }

        public async Task ComputeHashAsync(Func<IHasherPipeline> hasherPipelineBuilder)
        {
            ArgumentNullException.ThrowIfNull(hasherPipelineBuilder, nameof(hasherPipelineBuilder));
            
            if (_hash != null)
                return;

            // Recursively compute hash for each fork nodes.
            foreach (var fork in forks.Values)
                await fork.Node.ComputeHashAsync(hasherPipelineBuilder).ConfigureAwait(false);

            // Marshal current node, and set its hash.
            using var hasherPipeline = hasherPipelineBuilder();
            _hash = await hasherPipeline.HashDataAsync(ToByteArray()).ConfigureAwait(false);
            
            // Clean forks.
            forks.Clear();
        }

        // Helpers.
        private byte[] ForksToByteArray()
        {
            // Create a fork index of 32 bytes size, using each bit to represent the existence of the key.
            // Keys are ASCII chars in [0, 255], and can't be duplicated. We can map presence in a space of 32*8 bits.
            // After the index, write the serialized forks bytes.
            
            List<byte> bytes = [];
            
            //index
            var index = new byte[32];
            foreach (var k in forks.Keys)
                index[(byte)k / 8] |= (byte)(1 << (k % 8));
            
            bytes.AddRange(index);

            //forks
            foreach (var fork in forks.OrderBy(f => f.Key))
                bytes.AddRange(fork.Value.ToByteArray());

            return bytes.ToArray();
        }

        private byte[] HeaderToByteArray()
        {
            var headerBytes = new byte[NodeHeaderSize];
            
            Version02Hash.CopyTo(headerBytes.AsMemory()[..VersionHashSize]);
            headerBytes[VersionHashSize] = (byte)(skipWriteEntryHash ? 0 : SwarmHash.HashSize);
            
            return headerBytes;
        }

        private void RemoveNodeTypeFlag(NodeType flag) =>
            NodeTypeFlags &= ~flag;

        private void SetNodeTypeFlag(NodeType flag) =>
            NodeTypeFlags |= flag;
        
        private byte[] ToByteArray()
        {
            var bytes = new List<byte>();
            
            // Write obfuscation key.
            ObfuscationKey ??= XorEncryptKey.BuildNewRandom(); //generate obfuscation key if required
            bytes.AddRange(ObfuscationKey.Bytes.ToArray());
            
            // Write header.
            bytes.AddRange(HeaderToByteArray());

            // Write last entry hash.
            if (!skipWriteEntryHash)
                bytes.AddRange((EntryHash ?? SwarmHash.Zero).ToByteArray());

            // Write forks.
            bytes.AddRange(ForksToByteArray());

            // Obfuscate with key (except for key as first value).
            var bytesArray = bytes.ToArray();
            ObfuscationKey.EncryptDecrypt(bytesArray.AsSpan()[XorEncryptKey.KeySize..]);
            return bytesArray;
        }

        private void UpdateFlagIsWithPathSeparator(string path)
        {
            if (path.IndexOf(PathSeparator, StringComparison.InvariantCulture) > 0)
                SetNodeTypeFlag(NodeType.WithPathSeparator);
            else
                RemoveNodeTypeFlag(NodeType.WithPathSeparator);
        }
    }
}