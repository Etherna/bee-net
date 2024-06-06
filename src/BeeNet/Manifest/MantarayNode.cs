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
        public const int NodeHeaderSize = ObfuscationKey.KeySize + VersionHashSize + NodeRefBytesSize;
        public const int NodeRefBytesSize = 1;
        public const char PathSeparator = '/';
        public static readonly byte[] Version02Hash =
            Keccak256.ComputeHash("mantaray:0.2").Take(VersionHashSize).ToArray();
        public const int VersionHashSize = 31;
        
        // Fields.
        private readonly Dictionary<byte, MantarayNodeFork> forks = new();
        private ObfuscationKey? obfuscKey;

        public MantarayNode(bool isEncrypted)
        {
            IsEncrypted = isEncrypted;
            
            // Use empty obfuscation key if not encrypting.
            if (!isEncrypted)
                obfuscKey = ObfuscationKey.Empty;
        }

        // Properties.
        public byte[]? Address { get; private set; }
        public byte[]? Entry { get; private set; }
        public bool IsEncrypted { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();
        public NodeType NodeTypeFlags { get; private set; }
        public ObfuscationKey? ObfuscKey => obfuscKey;
        public int ReferenceBytesSize { get; private set; }

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));

            if (ReferenceBytesSize == 0)
            {
                // zero entry for directories
                if (entry.Address != SwarmAddress.Zero)
                    ReferenceBytesSize = SwarmAddress.HashSize;
            }

            if (path.Length == 0)
            {
                Entry = entry.Address.ToByteArray();
                SetNodeTypeFlag(NodeType.Value);
                if (entry.Metadata.Count > 0)
                {
                    Metadata = entry.Metadata;
                    SetNodeTypeFlag(NodeType.WithMetadata);
                }

                Address = null;
                return;
            }

            if (!forks.TryGetValue((byte)path[0], out var fork))
            {
                var nn = new MantarayNode(IsEncrypted);
                if (obfuscKey != null)
                    nn.obfuscKey = obfuscKey;
                nn.ReferenceBytesSize = ReferenceBytesSize;
                
                // check for prefix size limit
                if (path.Length > MantarayNodeFork.PrefixMaxSize)
                {
                    var prefix = path[..MantarayNodeFork.PrefixMaxSize];
                    var rest_ = path[MantarayNodeFork.PrefixMaxSize..];
                    nn.Add(rest_, entry);
                    nn.UpdateFlagIsWithPathSeparator(prefix);
                    forks[(byte)path[0]] = new MantarayNodeFork(prefix, nn);
                    SetNodeTypeFlag(NodeType.Edge);
                    return;
                }

                nn.Entry = entry.Address.ToByteArray();
                if (entry.Metadata.Count > 0)
                {
                    nn.Metadata = entry.Metadata;
                    nn.SetNodeTypeFlag(NodeType.WithMetadata);
                }
                nn.SetNodeTypeFlag(NodeType.Value);
                nn.UpdateFlagIsWithPathSeparator(path);
                forks[(byte)path[0]] = new MantarayNodeFork(path, nn);
                SetNodeTypeFlag(NodeType.Edge);
                return;
            }

            var common = fork.Prefix.FindCommonPrefix(path);
            var rest = fork.Prefix[common.Length..];
            var nn_ = fork.Node;
            if (rest.Length > 0)
            {
                // move current common prefix node
                nn_ = new MantarayNode(IsEncrypted);
                if (obfuscKey != null)
                    nn_.obfuscKey = obfuscKey;
                nn_.ReferenceBytesSize = ReferenceBytesSize;
                fork.Node.UpdateFlagIsWithPathSeparator(rest);
                nn_.forks[(byte)rest[0]] = new MantarayNodeFork(rest, fork.Node);
                nn_.SetNodeTypeFlag(NodeType.Edge);
                // if common path is full path new node is value type
                if (path.Length == common.Length)
                    nn_.SetNodeTypeFlag(NodeType.Value);
            }
            
	        // NOTE: special case on edge split
            nn_.UpdateFlagIsWithPathSeparator(path);
	        // add new for shared prefix
            nn_.Add(path[common.Length..], entry);
            forks[(byte)path[0]] = new MantarayNodeFork(common, nn_);
            SetNodeTypeFlag(NodeType.Edge);
        }

        public async Task ComputeAddressAsync(Func<IHasherPipeline> hasherPipelineBuilder)
        {
            ArgumentNullException.ThrowIfNull(hasherPipelineBuilder, nameof(hasherPipelineBuilder));
            
            if (Address != null)
                return;

            foreach (var fork in forks.Values)
                await fork.Node.ComputeAddressAsync(hasherPipelineBuilder).ConfigureAwait(false);

            var bytes = MarshalBinary();
            using var hasherPipeline = hasherPipelineBuilder();
            Address = (await hasherPipeline.HashDataAsync(bytes).ConfigureAwait(false)).ToByteArray();
            
            forks.Clear();
        }

        // Helpers.
        private static void Iterate(byte[] bytes, Func<byte, bool> byteEvaluator)
        {
            for (byte i = 0; ; i++)
            {
                if (((bytes[i / 8] >> (i % 8)) & 1) > 0 && !byteEvaluator(i))
                    throw new InvalidOperationException();
                if (i == 255) return;
            }
        }

        private byte[] MarshalBinary()
        {
            // Generate obfuscation key if required.
            obfuscKey ??= ObfuscationKey.BuildNewRandom();
            
            var bytes = new List<byte>();
            
            // header
            var headerBytes = new byte[NodeHeaderSize];
            
            obfuscKey.Bytes.CopyTo(headerBytes);
            Version02Hash.CopyTo(
                headerBytes.AsMemory()[ObfuscationKey.KeySize..(ObfuscationKey.KeySize + VersionHashSize)]);
            headerBytes[ObfuscationKey.KeySize + VersionHashSize] = (byte)ReferenceBytesSize;

            bytes.AddRange(headerBytes);

            // entry
            if (ReferenceBytesSize > 0)
            {
                var entryBytes = new byte[ReferenceBytesSize];
                Entry.CopyTo(entryBytes.AsSpan());
                bytes.AddRange(entryBytes);
            }

            // index
            var indexBytes = new byte[32];
            foreach (var k in forks.Keys)
                indexBytes[k / 8] |= (byte)(1 << (k % 8));
            
            bytes.AddRange(indexBytes);
            Iterate(indexBytes, b =>
            {
                var fork = forks[b];
                try
                {
                    Address = fork.Bytes();
                }
#pragma warning disable CA1031
                catch
#pragma warning restore CA1031
                {
                    return false;
                }
                bytes.AddRange(Address);
                return true;
            });
            
            // perform XOR encryption on bytes after obfuscation key
            var xorEncryptedBytes = new byte[bytes.Count];
            bytes.ToArray()[..ObfuscationKey.KeySize].CopyTo(xorEncryptedBytes.AsMemory());
            for (int i = ObfuscationKey.KeySize; i < bytes.Count; i += ObfuscationKey.KeySize)
            {
                var end = i + ObfuscationKey.KeySize;
                if (end > bytes.Count)
                    end = bytes.Count;

                var encrypted = obfuscKey.EncryptDecrypt(bytes.ToArray()[i..end]);
                encrypted.CopyTo(xorEncryptedBytes.AsMemory()[i..end]);
            }
            
            return xorEncryptedBytes;
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