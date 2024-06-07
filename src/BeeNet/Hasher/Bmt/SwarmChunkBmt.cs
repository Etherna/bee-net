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
using Nethereum.Merkle;
using Nethereum.Merkle.StrategyOptions.PairingConcat;
using Nethereum.Util.ByteArrayConvertors;
using Nethereum.Util.HashProviders;
using System;
using System.Collections.Generic;

namespace Etherna.BeeNet.Hasher.Bmt
{
    internal class SwarmChunkBmt : MerkleTree<byte[]>
    {
        // Classes.
        private class ChunkBmtByteArrayConvertor : IByteArrayConvertor<byte[]>
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
        private class HashProvider : IHashProvider
        {
            public byte[] ComputeHash(byte[] data) => Keccak256.ComputeHash(data);
        }
        
        // Consts.
        public const int MaxDataSize = SegmentsCount * SegmentSize;
        public const int SegmentsCount = 128;
        public const int SegmentSize = 32; //Keccak hash size
        
        // Static fields.
        private static readonly ChunkBmtByteArrayConvertor byteArrayConvertor = new();
        private static readonly HashProvider hashProvider = new();
        
        // Constructor.
        public SwarmChunkBmt()
            : base(hashProvider, byteArrayConvertor, PairingConcatType.Normal)
        { }
        
        // Static methods.
        public static byte[] ComputeHash(byte[] data) => hashProvider.ComputeHash(data);
        
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