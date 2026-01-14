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

using Etherna.BeeNet.Models;
using Newtonsoft.Json;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.Manifest
{
    public sealed class MantarayNodeFork
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
            MantarayNodeBase node)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(prefix);
            if (prefix.Length > PrefixMaxSize)
                throw new ArgumentOutOfRangeException(nameof(prefix));
            
            Prefix = prefix;
            Node = node;
        }

        // Properties.
        public string Prefix { get; }
        public MantarayNodeBase Node { get; }

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

            // Node reference.
            bytes.AddRange(Node.Reference.ToByteArray());

            // Metadata.
            if (Node.NodeTypeFlags.HasFlag(NodeType.WithMetadata))
            {
                var metadataBytes = new List<byte>();
                
                // Using Json encoding for metadata.
                metadataBytes.AddRange(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Node.Metadata)));
                var metadataTotalSize = metadataBytes.Count + MetadataBytesSize;
                
                // Pad bytes if necessary.
                if (metadataTotalSize % EncryptionKey256.KeySize != 0)
                {
                    var padding = new byte[EncryptionKey256.KeySize - metadataTotalSize % EncryptionKey256.KeySize];
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