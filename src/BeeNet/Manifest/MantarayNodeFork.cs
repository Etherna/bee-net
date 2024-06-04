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
        public const int NodeForkTypeBytesSize = 1;
        public const int NodeForkPrefixBytesSize = 1;
        public const int NodeForkHeaderSize = NodeForkTypeBytesSize + NodeForkPrefixBytesSize;
        public const int NodeForkPreReferenceSize = 32;
        public const int NodePrefixMaxSize = NodeForkPreReferenceSize - NodeForkHeaderSize;
        public const int NodeForkMetadataBytesSize = 2;
        
        // Constructor.
        public MantarayNodeFork(byte[] prefix,
            MantarayNode node)
        {
            Prefix = prefix;
            Node = node;
        }

        // Properties.
        /// <summary>
        /// the non-branching part of the subpath
        /// </summary>
        public byte[] Prefix { get; }

        /// <summary>
        /// in memory structure that represents the Node
        /// </summary>
        public MantarayNode Node { get; }

        public byte[] Bytes()
        {
            List<byte> b = new();
            
            var r = Node.Ref;
            // using 1 byte ('f.Node.refBytesSize') for size
            if (r!.Length > 256)
                throw new InvalidOperationException($"node reference size > 256: {r.Length}");
            
            b.Add(Node.NodeType);
            b.Add((byte)Prefix.Length);

            var prefixBytes = new byte[NodePrefixMaxSize];
            Prefix.CopyTo(prefixBytes.AsSpan());
            b.AddRange(prefixBytes);

            var refBytes = new byte[r.Length];
            r.CopyTo(refBytes.AsSpan());
            
            b.AddRange(refBytes);

            if (Node.IsWithMetadataType)
            {
                // using JSON encoding for metadata
                var metadataJson = JsonConvert.SerializeObject(Node.Metadata);
                var metadataJSONBytes = Encoding.UTF8.GetBytes(metadataJson);

                var metadataJSONBytesSizeWithSize = metadataJSONBytes.Length + NodeForkMetadataBytesSize;
                
                // pad JSON bytes if necessary
                if (metadataJSONBytesSizeWithSize < MantarayNode.NodeObfuscationKeySize)
                {
                    var paddingLength = MantarayNode.NodeObfuscationKeySize - metadataJSONBytesSizeWithSize;
                    var padding = new byte[paddingLength];
                    Array.Fill(padding, (byte)'\n');
                    metadataJSONBytes = metadataJSONBytes.Concat(padding).ToArray();
                }
                else if (metadataJSONBytesSizeWithSize > MantarayNode.NodeObfuscationKeySize)
                {
                    var paddingLength = MantarayNode.NodeObfuscationKeySize - metadataJSONBytesSizeWithSize % MantarayNode.NodeObfuscationKeySize;
                    var padding = new byte[paddingLength];
                    Array.Fill(padding, (byte)'\n');
                    metadataJSONBytes = metadataJSONBytes.Concat(padding).ToArray();
                }

                var metadataJSONBytesSize = metadataJSONBytes.Length;
                if (metadataJSONBytesSize > ushort.MaxValue)
                    throw new InvalidOperationException("metadata too large");

                var mBytesSize = new byte[NodeForkMetadataBytesSize];
                BinaryPrimitives.WriteUInt16BigEndian(mBytesSize, (ushort)metadataJSONBytesSize);
                
                b.AddRange(mBytesSize);
                b.AddRange(metadataJSONBytes);
            }
            
            return b.ToArray();
        }
    }
}