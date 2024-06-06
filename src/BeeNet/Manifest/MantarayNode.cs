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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
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
        private SwarmAddress? _address;
        private readonly Dictionary<char, MantarayNodeFork> forks = new();
        private bool lastEntryIsNotDirectory;

        public MantarayNode(XorEncryptKey? obfuscationKey)
        {
            ObfuscationKey = obfuscationKey;
        }

        // Properties.
        public SwarmAddress Address => _address ?? throw new InvalidOperationException("Address not computed");
        public SwarmAddress? EntryAddress { get; private set; }
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

            // Determine if the new entry is a directory.
            if (!entry.IsDirectory)
                lastEntryIsNotDirectory = true;

            // If the new entry doesn't have a path, this become a value node and directly take entry.
            if (path.Length == 0)
            {
                SetNodeTypeFlag(NodeType.Value);
                
                EntryAddress = entry.Address;
                if (entry.Metadata.Count > 0)
                {
                    Metadata = entry.Metadata;
                    SetNodeTypeFlag(NodeType.WithMetadata);
                }

                _address = null; //require to calculate address again.
                return;
            }

            // Else, if already exists a fork that contains the path.
            if (forks.TryGetValue(path[0], out var fork))
            {
                var commonPrefix = fork.Prefix.FindCommonPrefix(path);
                var rest = fork.Prefix[commonPrefix.Length..];
                var nn = fork.Node;
                if (rest.Length > 0)
                {
                    // move current common prefix node
                    nn = new MantarayNode(ObfuscationKey)
                    {
                        lastEntryIsNotDirectory = lastEntryIsNotDirectory
                    };
                    fork.Node.UpdateFlagIsWithPathSeparator(rest);
                    nn.forks[rest[0]] = new MantarayNodeFork(rest, fork.Node);
                    nn.SetNodeTypeFlag(NodeType.Edge);
                    // if common path is full path new node is value type
                    if (path.Length == commonPrefix.Length)
                        nn.SetNodeTypeFlag(NodeType.Value);
                }
            
                // NOTE: special case on edge split
                nn.UpdateFlagIsWithPathSeparator(path);
                // add new for shared prefix
                nn.Add(path[commonPrefix.Length..], entry);
                forks[path[0]] = new MantarayNodeFork(commonPrefix, nn);
                SetNodeTypeFlag(NodeType.Edge);
            }

            // Else, create the new fork.
            else
            {
                var nn = new MantarayNode(ObfuscationKey)
                {
                    lastEntryIsNotDirectory = lastEntryIsNotDirectory
                };

                // check for prefix size limit
                if (path.Length > MantarayNodeFork.PrefixMaxSize)
                {
                    var prefix = path[..MantarayNodeFork.PrefixMaxSize];
                    var rest = path[MantarayNodeFork.PrefixMaxSize..];
                    nn.Add(rest, entry);
                    nn.UpdateFlagIsWithPathSeparator(prefix);
                    forks[path[0]] = new MantarayNodeFork(prefix, nn);
                    SetNodeTypeFlag(NodeType.Edge);
                    return;
                }

                nn.EntryAddress = entry.Address;
                if (entry.Metadata.Count > 0)
                {
                    nn.Metadata = entry.Metadata;
                    nn.SetNodeTypeFlag(NodeType.WithMetadata);
                }
                nn.SetNodeTypeFlag(NodeType.Value);
                nn.UpdateFlagIsWithPathSeparator(path);
                forks[path[0]] = new MantarayNodeFork(path, nn);
                SetNodeTypeFlag(NodeType.Edge);
            }
        }

        public async Task ComputeAddressAsync(Func<IHasherPipeline> hasherPipelineBuilder)
        {
            ArgumentNullException.ThrowIfNull(hasherPipelineBuilder, nameof(hasherPipelineBuilder));
            
            if (_address != null)
                return;

            // Recursively compute address for each fork nodes.
            foreach (var fork in forks.Values)
                await fork.Node.ComputeAddressAsync(hasherPipelineBuilder).ConfigureAwait(false);

            // Marshal current node, and set address as its hash.
            using var hasherPipeline = hasherPipelineBuilder();
            _address = await hasherPipeline.HashDataAsync(MarshalBinary()).ConfigureAwait(false);
            
            // Clean forks.
            forks.Clear();
        }

        // Helpers.
        private byte[] MarshalBinary()
        {
            var bytes = new List<byte>();
            
            // Write obfuscation key.
            ObfuscationKey ??= XorEncryptKey.BuildNewRandom(); //generate obfuscation key if required
            bytes.AddRange(ObfuscationKey.Bytes.ToArray());
            
            // Write header.
            bytes.AddRange(MarshalHeader());

            // Write last entry address.
            if (lastEntryIsNotDirectory)
                bytes.AddRange((EntryAddress ?? SwarmAddress.Zero).ToByteArray());

            // Write forks.
            bytes.AddRange(MarshalForks());

            // Obfuscate with key (except for key as first value).
            var bytesArray = bytes.ToArray();
            ObfuscationKey.EncryptDecrypt(bytesArray.AsSpan()[XorEncryptKey.KeySize..]);
            return bytesArray;
        }

        private byte[] MarshalForks()
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
                bytes.AddRange(fork.Value.Bytes());

            return bytes.ToArray();
        }

        private byte[] MarshalHeader()
        {
            var headerBytes = new byte[NodeHeaderSize];
            
            Version02Hash.CopyTo(headerBytes.AsMemory()[..VersionHashSize]);
            headerBytes[VersionHashSize] = (byte)(lastEntryIsNotDirectory ? SwarmAddress.HashSize : 0);
            
            return headerBytes;
        }

        private void RemoveNodeTypeFlag(NodeType flag) =>
            NodeTypeFlags &= ~flag;

        private void SetNodeTypeFlag(NodeType flag) =>
            NodeTypeFlags |= flag;

        private void UpdateFlagIsWithPathSeparator(string path)
        {
            if (path.IndexOf(PathSeparator, StringComparison.InvariantCulture) > 0)
                SetNodeTypeFlag(NodeType.WithPathSeparator);
            else
                RemoveNodeTypeFlag(NodeType.WithPathSeparator);
        }
    }
}