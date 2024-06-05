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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class MantarayNode
    {
        // Consts.
        public const int NodeHeaderSize = ObfuscationKeySize + VersionHashSize + NodeRefBytesSize;
        public const int NodeRefBytesSize = 1;
        public const int ObfuscationKeySize = 32;
        public const char PathSeparator = '/';
        public static readonly byte[] Version02Hash =
            Keccak256.ComputeHash("mantaray:0.2").Take(VersionHashSize).ToArray();
        public const int VersionHashSize = 31;
        
        // Fields.
        private readonly Dictionary<byte, MantarayNodeFork> _forks = new();
        private byte[]? _obfuscationKey;

        public MantarayNode(bool isEncrypted)
        {
            IsEncrypted = isEncrypted;
            
            // Use empty obfuscation key if not encrypting.
            if (!isEncrypted)
                _obfuscationKey = new byte[ObfuscationKeySize];
        }

        // Properties.
        public byte[]? Entry { get; private set; }
        public IReadOnlyDictionary<byte, MantarayNodeFork> Forks => _forks;
        public bool IsEncrypted { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();
        public NodeType NodeTypeFlags { get; private set; }
        public ReadOnlyMemory<byte> ObfuscationKey => _obfuscationKey;
        
        /// <summary>
        /// reference to uninstantiated Node persisted serialised
        /// </summary>
        public byte[]? Reference { get; private set; }
        public int ReferenceBytesSize { get; private set; }
        
        // Static properties.
        public static ReadOnlyMemory<byte> ZeroObfuscationKey { get; } = new byte[ObfuscationKeySize];

        // Methods.
        public void Add(byte[] path, byte[] entry, IReadOnlyDictionary<string, string> metadata)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));
            ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));

            if (ReferenceBytesSize == 0)
            {
                if (entry.Length > 256)
                    throw new ArgumentOutOfRangeException(nameof(entry), $"Node entry size > 256: {entry.Length}");

                // zero entry for directories
                if (entry != SwarmAddress.Zero)
                    ReferenceBytesSize = entry.Length;
            }
            else if (entry.Length > 0 && ReferenceBytesSize != entry.Length)
                throw new InvalidOperationException($"Invalid entry size: {entry.Length}, expected: {ReferenceBytesSize}");

            if (path.Length == 0)
            {
                Entry = entry;
                SetNodeTypeFlag(NodeType.Value);
                if (metadata.Count > 0)
                {
                    Metadata = metadata;
                    SetNodeTypeFlag(NodeType.WithMetadata);
                }

                Reference = null;
                return;
            }

            if (!Forks.TryGetValue(path[0], out var fork))
            {
                var nn = new MantarayNode(IsEncrypted);
                if (_obfuscationKey?.Length > 0)
                    nn._obfuscationKey = _obfuscationKey;
                nn.ReferenceBytesSize = ReferenceBytesSize;
                
                // check for prefix size limit
                if (path.Length > MantarayNodeFork.PrefixMaxSize)
                {
                    var prefix = path[..MantarayNodeFork.PrefixMaxSize];
                    var rest_ = path[MantarayNodeFork.PrefixMaxSize..];
                    nn.Add(rest_, entry, metadata);
                    nn.UpdateFlagIsWithPathSeparator(prefix);
                    _forks[path[0]] = new MantarayNodeFork(prefix, nn);
                    SetNodeTypeFlag(NodeType.Edge);
                    return;
                }

                nn.Entry = entry;
                if (metadata.Count > 0)
                {
                    nn.Metadata = metadata;
                    nn.SetNodeTypeFlag(NodeType.WithMetadata);
                }
                nn.SetNodeTypeFlag(NodeType.Value);
                nn.UpdateFlagIsWithPathSeparator(path);
                _forks[path[0]] = new MantarayNodeFork(path, nn);
                SetNodeTypeFlag(NodeType.Edge);
                return;
            }

            var common = fork.Prefix.ToArray().FindCommonPrefixWith(path);
            var rest = fork.Prefix[common.Length..].ToArray();
            var nn_ = fork.Node;
            if (rest.Length > 0)
            {
                // move current common prefix node
                nn_ = new MantarayNode(IsEncrypted);
                if (_obfuscationKey?.Length > 0)
                    nn_._obfuscationKey = _obfuscationKey;
                nn_.ReferenceBytesSize = ReferenceBytesSize;
                fork.Node.UpdateFlagIsWithPathSeparator(rest);
                nn_._forks[rest[0]] = new MantarayNodeFork(rest, fork.Node);
                nn_.SetNodeTypeFlag(NodeType.Edge);
                // if common path is full path new node is value type
                if (path.Length == common.Length)
                    nn_.SetNodeTypeFlag(NodeType.Value);
            }
            
	        // NOTE: special case on edge split
            nn_.UpdateFlagIsWithPathSeparator(path);
	        // add new for shared prefix
            nn_.Add(path[common.Length..], entry, metadata);
            _forks[path[0]] = new MantarayNodeFork(common, nn_);
            SetNodeTypeFlag(NodeType.Edge);
        }

        public async Task SaveAsync(Func<IHasherPipeline> hasherPipelineBuilder)
        {
            ArgumentNullException.ThrowIfNull(hasherPipelineBuilder, nameof(hasherPipelineBuilder));
            
            if (Reference != null)
                return;

            foreach (var fork in Forks.Values)
                await fork.Node.SaveAsync(hasherPipelineBuilder).ConfigureAwait(false);

            var bytes = MarshalBinary();
            using var hasherPipeline = hasherPipelineBuilder();
            Reference = (await hasherPipeline.HashDataAsync(bytes).ConfigureAwait(false)).ToByteArray();
            
            _forks.Clear();
        }

        // Helpers.
        /// <summary>
        /// encryptDecrypt runs a XOR encryption on the input bytes, encrypting it if it
        /// hasn't already been, and decrypting it if it has, using the key provided.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static byte[] EncryptDecrypt(byte[] input, byte[] key)
        {
            var output = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = (byte)(input[i] ^ key[i % key.Length]);

            return output;
        }

        private static bool GetUint8(byte[] bb, byte i) =>
            ((bb[i / 8] >> (i % 8)) & 1) > 0;

        private static void Iter(byte[] bb, Func<byte, bool> f)
        {
            for (byte i = 0; ; i++)
            {
                if (GetUint8(bb, i) && !f(i))
                    throw new InvalidOperationException();
                if (i == 255) return;
            }
        }

        private byte[] MarshalBinary()
        {
            // Generate obfuscation key if required.
            if (_obfuscationKey is null)
                RandomNumberGenerator.Fill(_obfuscationKey);
            
            var bytes = new List<byte>();
            
            // header
            var headerBytes = new byte[NodeHeaderSize];
            
            _obfuscationKey.AsSpan().CopyTo(headerBytes);
            Version02Hash.CopyTo(
                headerBytes.AsMemory()[ObfuscationKeySize..(ObfuscationKeySize + VersionHashSize)]);
            headerBytes[ObfuscationKeySize + VersionHashSize] = (byte)ReferenceBytesSize;

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
            foreach (var k in Forks.Keys)
                indexBytes[k / 8] |= (byte)(1 << (k % 8));
            
            bytes.AddRange(indexBytes);
            Iter(indexBytes, b =>
            {
                var f = Forks[b];
                try
                {
                    Reference = f.Bytes();
                }
#pragma warning disable CA1031
                catch
#pragma warning restore CA1031
                {
                    return false;
                }
                bytes.AddRange(Reference);
                return true;
            });
            
            // perform XOR encryption on bytes after obfuscation key
            var xorEncryptedBytes = new byte[bytes.Count];
            bytes.ToArray()[..ObfuscationKeySize].CopyTo(xorEncryptedBytes.AsMemory());
            for (int i = ObfuscationKeySize; i < bytes.Count; i += ObfuscationKeySize)
            {
                var end = i + ObfuscationKeySize;
                if (end > bytes.Count)
                    end = bytes.Count;

                var encrypted = EncryptDecrypt(bytes.ToArray()[i..end], _obfuscationKey!);
                encrypted.CopyTo(xorEncryptedBytes.AsMemory()[i..end]);
            }
            
            return xorEncryptedBytes;
        }

        private void RemoveNodeTypeFlag(NodeType flag) =>
            NodeTypeFlags &= ~flag;

        private void SetNodeTypeFlag(NodeType flag) =>
            NodeTypeFlags |= flag;

        private void UpdateFlagIsWithPathSeparator(byte[] path)
        {
            if (Array.FindIndex(path, b => b == PathSeparator) > 0)
                SetNodeTypeFlag(NodeType.WithPathSeparator);
            else
                RemoveNodeTypeFlag(NodeType.WithPathSeparator);
        }
    }
}