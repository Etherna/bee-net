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
using Nethereum.Merkle;
using Nethereum.Merkle.StrategyOptions.PairingConcat;
using Nethereum.Util.ByteArrayConvertors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Hashing
{
    internal sealed class SwarmChunkBmt(IHasher hasher)
        : MerkleTree<byte[]>(
            hasher,
            byteArrayConvertor,
            PairingConcatType.Normal)
    {
        // Classes.
        private sealed class ChunkBmtByteArrayConvertor : IByteArrayConvertor<byte[]>
        {
            /// <summary>
            /// Verify that chunk segment data has right size
            /// </summary>
            /// <param name="data">Input raw data</param>
            /// <returns>Leaf data</returns>
            public byte[] ConvertToByteArray(byte[] data)
            {
                ArgumentNullException.ThrowIfNull(data, nameof(data));

                if (data.Length == SegmentSize)
                    return data;
                if (data.Length > SegmentSize)
                    throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {SegmentSize}");
            
                Array.Resize(ref data, SegmentSize);
                return data;
            }
        }
        
        // Consts.
        public const int SegmentsCount = SwarmChunk.DataSize / SegmentSize;
        public const int SegmentSize = SwarmHash.HashSize;
        
        // Static fields.
        private static readonly ChunkBmtByteArrayConvertor byteArrayConvertor = new();
        
        // Methods.
        public SwarmHash Hash(byte[] span, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            if (data.Length > SwarmChunk.DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Max writable data is {SwarmChunk.DataSize} bytes");
            
            // Split input data into leaf segments.
            var segments = new List<byte[]>();
            for (var start = 0; start < data.Length; start += SegmentSize)
            {
                var end = Math.Min(start + SegmentSize, data.Length);
                segments.Add(data[start..end]);
            }
            
            // Build the merkle tree.
            BuildTree(segments);
            
            return hasher.ComputeHash(span.Concat(Root.Hash).ToArray());
        }
        
        // Protected override methods.
        protected override MerkleTreeNode CreateMerkleTreeNode(byte[] item) =>
            new(byteArrayConvertor.ConvertToByteArray(item));

        protected override void InitialiseLeavesAndLayersAndBuildTree(List<MerkleTreeNode> leaves)
        {
            ArgumentNullException.ThrowIfNull(leaves, nameof(leaves));
            
            // Add missing empty leaves.
            while (leaves.Count < SegmentsCount)
                leaves.Add(new MerkleTreeNode(new byte[SegmentSize]));
            
            base.InitialiseLeavesAndLayersAndBuildTree(leaves);
        }
    }
}