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

using Etherna.BeeNet.Models;
using Newtonsoft.Json;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Etherna.BeeNet.Manifest
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class MantarayNodeFork
    {
        // Consts.
        public const int HeaderSize = TypeSize + PrefixSize;
        public const int MetadataBytesSize = 2;
        public const int PrefixMaxSize = PreReferenceSize - HeaderSize;
        public const int PrefixSize = 1;
        public const int PreReferenceSize = SwarmAddress.HashSize;
        public const int TypeSize = 1;
        
        // Constructor.
        public MantarayNodeFork(
            string prefix,
            MantarayNode node)
        {
            ArgumentNullException.ThrowIfNull(node, nameof(node));
            ArgumentNullException.ThrowIfNull(prefix, nameof(prefix));
            if (prefix.Length > PrefixMaxSize)
                throw new ArgumentOutOfRangeException(nameof(prefix));
            
            Prefix = prefix;
            Node = node;
        }

        // Properties.
        /// <summary>
        /// The non-branching part of the subpath
        /// </summary>
        public string Prefix { get; }
        public MantarayNode Node { get; }

        public byte[] Bytes()
        {
            List<byte> b = new();
            
            var r = Node.Reference;
            // using 1 byte ('f.Node.refBytesSize') for size
            if (r!.Length > 256)
                throw new InvalidOperationException($"node reference size > 256: {r.Length}");
            
            b.Add((byte)Node.NodeTypeFlags);
            b.Add((byte)Prefix.Length);

            var prefixBytes = new byte[PrefixMaxSize];
            Encoding.UTF8.GetBytes(Prefix).CopyTo(prefixBytes.AsSpan());
            b.AddRange(prefixBytes);

            var refBytes = new byte[r.Length];
            r.CopyTo(refBytes.AsSpan());
            
            b.AddRange(refBytes);

            if (Node.NodeTypeFlags.HasFlag(NodeType.WithMetadata))
            {
                // using JSON encoding for metadata
                var metadataJson = JsonConvert.SerializeObject(Node.Metadata);
                var metadataJSONBytes = Encoding.UTF8.GetBytes(metadataJson);

                var metadataJSONBytesSizeWithSize = metadataJSONBytes.Length + MetadataBytesSize;
                
                // pad JSON bytes if necessary
                if (metadataJSONBytesSizeWithSize < MantarayNode.ObfuscationKeySize)
                {
                    var paddingLength = MantarayNode.ObfuscationKeySize - metadataJSONBytesSizeWithSize;
                    var padding = new byte[paddingLength];
                    Array.Fill(padding, (byte)'\n');
                    metadataJSONBytes = metadataJSONBytes.Concat(padding).ToArray();
                }
                else if (metadataJSONBytesSizeWithSize > MantarayNode.ObfuscationKeySize)
                {
                    var paddingLength = MantarayNode.ObfuscationKeySize - metadataJSONBytesSizeWithSize % MantarayNode.ObfuscationKeySize;
                    var padding = new byte[paddingLength];
                    Array.Fill(padding, (byte)'\n');
                    metadataJSONBytes = metadataJSONBytes.Concat(padding).ToArray();
                }

                var metadataJSONBytesSize = metadataJSONBytes.Length;
                if (metadataJSONBytesSize > ushort.MaxValue)
                    throw new InvalidOperationException("metadata too large");

                var mBytesSize = new byte[MetadataBytesSize];
                BinaryPrimitives.WriteUInt16BigEndian(mBytesSize, (ushort)metadataJSONBytesSize);
                
                b.AddRange(mBytesSize);
                b.AddRange(metadataJSONBytes);
            }
            
            return b.ToArray();
        }
    }
}