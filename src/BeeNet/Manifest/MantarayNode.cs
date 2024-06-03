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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        public const byte NodeTypeValue = 2;
        public const byte NodeTypeEdge = 4;
        public const byte NodeTypeWithPathSeparator = 8;
        public const byte NodeTypeWithMetadata = 16;
        public const byte NodeTypeMask = 255;
        public const int NodePrefixMaxSize = NodeForkPreReferenceSize - NodeForkHeaderSize;
        public const int ObfuscationKeySize = 32;
        public const char PathSeparator = '/';
        
        // Properties.
        public byte[]? Entry { get; private set; }
        public Dictionary<byte, MantarayNodeFork?> Forks { get; } = new();
        public Dictionary<string, string> Metadata { get; private set; } = new();
        public byte NodeType { get; private set; }
        public ReadOnlyMemory<byte> ObfuscationKey { get; set; }
        
        /// <summary>
        /// reference to uninstantiated Node persisted serialised
        /// </summary>
        public byte[]? Ref { get; private set; }
        public int RefBytesSize { get; private set; }
        
        // Static properties.
        public static ReadOnlyMemory<byte> ZeroObfuscationKey { get; } = new byte[ObfuscationKeySize];
        
        // Methods.
        public void Add(byte[] path, byte[]? entry, Dictionary<string, string> metadata, IHasherPipeline hasherPipeline)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));
            
            entry ??= Array.Empty<byte>();

            if (RefBytesSize == 0)
            {
                if (entry.Length > 256)
                    throw new ArgumentOutOfRangeException(nameof(entry), $"node entry size > 256: {entry.Length}");

                // empty entry for directories
                if (entry.Length > 0)
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

            var f = Forks![path[0]];
            if (f == null)
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

        // Helpers.
        private static byte[] Common(byte[] a, byte[] b) =>
            a.TakeWhile((ab, i) => b[i] == ab).ToArray();
        
        private void MakeEdge() => NodeType |= NodeTypeEdge;

        private void MakeNotWithPathSeparator() => NodeType &= NodeTypeMask ^ NodeTypeWithPathSeparator;

        private void MakeValue() => NodeType |= NodeTypeValue;

        private void MakeWithMetadata() => NodeType |= NodeTypeWithMetadata;

        private void MakeWithPathSeparator() => NodeType |= NodeTypeWithPathSeparator;

        private void UpdateIsWithPathSeparator(byte[] path)
        {
            if (Array.FindIndex(path, b => b == PathSeparator) > 0)
                MakeWithPathSeparator();
            else
                MakeNotWithPathSeparator();
        }
    }
}