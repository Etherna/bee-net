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

using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Models;
using Nethereum.Hex.HexConvertors.Extensions;
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
        public const int NodeForkTypeBytesSize = 1;
        public const int NodeForkPrefixBytesSize = 1;
        public const int NodeForkHeaderSize = NodeForkTypeBytesSize + NodeForkPrefixBytesSize;
        public const int NodeForkMetadataBytesSize = 2;
        public const int NodeForkPreReferenceSize = 32;
        public const int NodeHeaderSize = NodeObfuscationKeySize + VersionHashSize + NodeRefBytesSize;
        public const byte NodeTypeValue = 2;
        public const byte NodeTypeEdge = 4;
        public const byte NodeTypeWithPathSeparator = 8;
        public const byte NodeTypeWithMetadata = 16;
        public const byte NodeTypeMask = 255;
        public const int NodeObfuscationKeySize = 32;
        public const int NodePrefixMaxSize = NodeForkPreReferenceSize - NodeForkHeaderSize;
        public const int NodeRefBytesSize = 1;
        public const int ObfuscationKeySize = 32;
        public const char PathSeparator = '/';
        public const int VersionHashSize = 31;

        public const string VersionNameString = "mantaray";
        public const string VersionCode01String = "0.1";
        public const string VersionCode02String = "0.2";
        public const string VersionSeparatorString = ":";

        public const string Version01String = VersionNameString + VersionSeparatorString + VersionCode01String;   // "mantaray:0.1"
        public const string Version01HashString = "025184789d63635766d78c41900196b57d7400875ebe4d9b5d1e76bd9652a9b7"; // pre-calculated version string, Keccak-256

        public const string Version02String = VersionNameString + VersionSeparatorString + VersionCode02String;   // "mantaray:0.2"
        public const string Version02HashString = "5768b3b6a7db56d21d1abff40d41cebfc83448fed8d7e9b06ec0d3b073f28f7b"; // pre-calculated version string, Keccak-256
        
        // Fields.
        private byte[]? _obfuscationKey;
        
        // Properties.
        public byte[]? Entry { get; private set; }
        public Dictionary<byte, MantarayNodeFork> Forks { get; } = new();
        public bool IsWithMetadataType => (NodeType & NodeTypeWithMetadata) == NodeTypeWithMetadata;
        public Dictionary<string, string> Metadata { get; private set; } = new();
        public byte NodeType { get; private set; }

        public ReadOnlyMemory<byte> ObfuscationKey
        {
            get => _obfuscationKey;
            set => _obfuscationKey = value.ToArray();
        }
        
        /// <summary>
        /// reference to uninstantiated Node persisted serialised
        /// </summary>
        public byte[]? Ref { get; private set; }
        public int RefBytesSize { get; private set; }
        
        // Static properties.
        public static ReadOnlyMemory<byte> ZeroObfuscationKey { get; } = new byte[ObfuscationKeySize];
        public static byte[] Version01HashBytes => Version01HashString.HexToByteArray();
        public static byte[] Version02HashBytes => Version02HashString.HexToByteArray();

        // Methods.
        public void Add(byte[] path, byte[] entry, Dictionary<string, string> metadata, IHasherPipeline hasherPipeline)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));
            ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));

            if (RefBytesSize == 0)
            {
                if (entry.Length > 256)
                    throw new ArgumentOutOfRangeException(nameof(entry), $"node entry size > 256: {entry.Length}");

                // zero entry for directories
                if (entry != SwarmAddress.Zero)
                    RefBytesSize = entry.Length;
            }
            else if (entry.Length > 0 && RefBytesSize != entry.Length)
                throw new InvalidOperationException($"invalid entry size: {entry.Length}, expected: {RefBytesSize}");

            if (path.Length == 0)
            {
                Entry = entry;
                MakeValue();
                if (metadata.Count > 0)
                {
                    Metadata = metadata;
                    MakeWithMetadata();
                }

                Ref = null;
                return;
            }

            if (!Forks.TryGetValue(path[0], out var f))
            {
                var nn = new MantarayNode();
                if (ObfuscationKey.Length > 0)
                    nn.ObfuscationKey = ObfuscationKey;
                nn.RefBytesSize = RefBytesSize;
                
                // check for prefix size limit
                if (path.Length > NodePrefixMaxSize)
                {
                    var prefix = path[..NodePrefixMaxSize];
                    var rest_ = path[NodePrefixMaxSize..];
                    nn.Add(rest_, entry, metadata, hasherPipeline);
                    nn.UpdateIsWithPathSeparator(prefix);
                    Forks[path[0]] = new MantarayNodeFork(prefix, nn);
                    MakeEdge();
                    return;
                }

                nn.Entry = entry;
                if (metadata.Count > 0)
                {
                    nn.Metadata = metadata;
                    nn.MakeWithMetadata();
                }
                nn.MakeValue();
                nn.UpdateIsWithPathSeparator(path);
                Forks[path[0]] = new MantarayNodeFork(path, nn);
                MakeEdge();
                return;
            }

            var c = Common(f.Prefix, path);
            var rest = f.Prefix[c.Length..];
            var nn_ = f.Node;
            if (rest.Length > 0)
            {
                // move current common prefix node
                nn_ = new MantarayNode();
                if (ObfuscationKey.Length > 0)
                    nn_.ObfuscationKey = ObfuscationKey;
                nn_.RefBytesSize = RefBytesSize;
                f.Node.UpdateIsWithPathSeparator(rest);
                nn_.Forks[rest[0]] = new MantarayNodeFork(rest, f.Node);
                nn_.MakeEdge();
                // if common path is full path new node is value type
                if (path.Length == c.Length)
                    nn_.MakeValue();
            }
            
	        // NOTE: special case on edge split
            nn_.UpdateIsWithPathSeparator(path);
	        // add new for shared prefix
            nn_.Add(path[c.Length..], entry, metadata, hasherPipeline);
            Forks[path[0]] = new MantarayNodeFork(c, nn_);
            MakeEdge();
        }

        public async Task SaveAsync(IHasherPipeline hasherPipeline)
        {
            ArgumentNullException.ThrowIfNull(hasherPipeline, nameof(hasherPipeline));
            
            if (Ref != null)
                return;

            foreach (var fork in Forks.Values)
                await fork.Node.SaveAsync(hasherPipeline).ConfigureAwait(false);

            var bytes = MarshalBinary();
            Ref = (await hasherPipeline.HashDataAsync(bytes).ConfigureAwait(false)).ToByteArray();
            
            Forks.Clear();
        }

        // Helpers.
        private static byte[] Common(byte[] a, byte[] b) =>
            a.TakeWhile((ab, i) => b[i] == ab).ToArray();

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
                if (GetUint8(bb, i) && f(i))
                    throw new InvalidOperationException();
                if (i == 255) return;
            }
        }
        
        private void MakeEdge() => NodeType |= NodeTypeEdge;

        private void MakeNotWithPathSeparator() => NodeType &= NodeTypeMask ^ NodeTypeWithPathSeparator;

        private void MakeValue() => NodeType |= NodeTypeValue;

        private void MakeWithMetadata() => NodeType |= NodeTypeWithMetadata;

        private void MakeWithPathSeparator() => NodeType |= NodeTypeWithPathSeparator;

        private byte[] MarshalBinary()
        {
            var bytes = new List<byte>();
            
            // header
            var headerBytes = new byte[NodeHeaderSize];

            //generate obfuscation key
            if (ObfuscationKey.Length == 0)
                RandomNumberGenerator.Fill(_obfuscationKey);
            
            ObfuscationKey.CopyTo(headerBytes.AsMemory()[..NodeObfuscationKeySize]);
            Version02HashBytes.CopyTo(
                headerBytes.AsMemory()[NodeObfuscationKeySize..(NodeObfuscationKeySize + VersionHashSize)]);
            headerBytes[NodeObfuscationKeySize + VersionHashSize] = (byte)RefBytesSize;

            bytes.AddRange(headerBytes);

            // entry
            var entryBytes = new byte[RefBytesSize];
            Entry.CopyTo(entryBytes.AsSpan());
            bytes.AddRange(entryBytes);

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
                    Ref = f.Bytes();
                }
#pragma warning disable CA1031
                catch
#pragma warning restore CA1031
                {
                    return false;
                }
                bytes.AddRange(Ref);
                return true;
            });
            
            // perform XOR encryption on bytes after obfuscation key
            var xorEncryptedBytes = new byte[bytes.Count];
            bytes.ToArray()[..NodeObfuscationKeySize].CopyTo(xorEncryptedBytes.AsMemory());
            for (int i = NodeObfuscationKeySize; i < bytes.Count; i += NodeObfuscationKeySize)
            {
                var end = i + NodeObfuscationKeySize;
                if (end > bytes.Count)
                    end = bytes.Count;

                var encrypted = EncryptDecrypt(bytes.ToArray()[i..end], _obfuscationKey!);
                encrypted.CopyTo(xorEncryptedBytes.AsMemory()[i..end]);
            }
            
            return xorEncryptedBytes;
        }

        private void UpdateIsWithPathSeparator(byte[] path)
        {
            if (Array.FindIndex(path, b => b == PathSeparator) > 0)
                MakeWithPathSeparator();
            else
                MakeNotWithPathSeparator();
        }
    }
}