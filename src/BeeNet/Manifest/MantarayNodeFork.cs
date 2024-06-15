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
using System.Text;

namespace Etherna.BeeNet.Manifest
{
    public class MantarayNodeFork
    {
        // Consts.
        public const int HeaderSize = TypeSize + PrefixSize;
        public const int MetadataBytesSize = 2;
        public const int PrefixMaxSize = SwarmHash.HashSize - HeaderSize;
        public const int PrefixSize = 1;
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
        public string Prefix { get; }
        public MantarayNode Node { get; }

        // Methods.
        public byte[] ToByteArray()
        {
            // Header.
            List<byte> bytes =
            [
                (byte)Node.NodeTypeFlags,
                (byte)Prefix.Length
            ];
            
            // Prefix.
            var prefixBytes = new byte[PrefixMaxSize];
            Encoding.UTF8.GetBytes(Prefix).CopyTo(prefixBytes.AsSpan());
            bytes.AddRange(prefixBytes);

            // Node hash.
            bytes.AddRange(Node.Hash.ToByteArray());

            // Metadata.
            if (Node.NodeTypeFlags.HasFlag(NodeType.WithMetadata))
            {
                var metadataBytes = new List<byte>();
                
                // Using Json encoding for metadata.
                metadataBytes.AddRange(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Node.Metadata)));
                var metadataTotalSize = metadataBytes.Count + MetadataBytesSize;
                
                // Pad bytes if necessary.
                if (metadataTotalSize % XorEncryptKey.KeySize != 0)
                {
                    var padding = new byte[XorEncryptKey.KeySize - metadataTotalSize % XorEncryptKey.KeySize];
                    Array.Fill(padding, (byte)'\n');
                    metadataBytes.AddRange(padding);
                }

                // Add metadata.
                if (metadataBytes.Count > ushort.MaxValue)
                    throw new InvalidOperationException("metadata too large");
                
                var sizeBytes = new byte[MetadataBytesSize];
                BinaryPrimitives.WriteUInt16BigEndian(sizeBytes, (ushort)metadataBytes.Count);
                
                bytes.AddRange(sizeBytes);
                bytes.AddRange(metadataBytes);
            }
            
            return bytes.ToArray();
        }
    }
}