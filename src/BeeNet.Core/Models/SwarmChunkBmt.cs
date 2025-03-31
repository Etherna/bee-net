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

using Etherna.BeeNet.Hashing;
using Nethereum.Merkle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmChunkBmt(IHasher hasher) : ISwarmChunkBmt
    {
        // Consts.
        public const int SegmentsCount = SwarmChunk.DataSize / SegmentSize;
        public const int SegmentSize = SwarmHash.HashSize;
        
        // Fields.
        private List<List<MerkleTreeNode>> layers = [];
        private List<MerkleTreeNode> leaves = [];
        private readonly Queue<MerkleTreeNode> merkleTreeNodesPool = new();
        private MerkleTreeNode? root;
        
        // Properties.
        public IHasher Hasher => hasher;
        public MerkleTreeNode? Root => root;

        // Methods.
        public void Clear()
        {
            foreach (var merkleTreeNode in layers.SelectMany(l => l))
                merkleTreeNodesPool.Enqueue(merkleTreeNode);
            layers.Clear();
            leaves.Clear();
            root = null;
        }
        
        public IReadOnlyCollection<byte[]> GetProof(byte[] chunkSegment)
        {
            var hashLeaf = hasher.ComputeHash(ChunkSegmentToLeafByteArray(chunkSegment));

            for (var i = 0; i < leaves.Count; i++)
                if (leaves[i].Matches(hashLeaf))
                    return GetProof(i);

            throw new KeyNotFoundException("Leaf not found");
        }

        public IReadOnlyCollection<byte[]> GetProof(int index)
        {
            var proofs = new List<byte[]>();
            for (var i = 0; i < layers.Count; i++)
            {
                var isRightNode = index % 2 == 1;
                var pairIndex = isRightNode ? index - 1 : index + 1;
                var currentLayer = layers[i];
                if (pairIndex < currentLayer.Count)
                    proofs.Add(currentLayer[pairIndex].Hash);

                index = (index / 2) | 0;
            }

            return proofs;
        }

        public SwarmHash Hash(byte[] span, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            if (data.Length > SwarmChunk.DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Max writable data is {SwarmChunk.DataSize} bytes");
            
            // Split input data into leaf segments.
            var segments = new List<byte[]>();
            for (var start = 0; start < data.Length; start += SegmentSize)
            {
                var end = Math.Min(start + SegmentSize, data.Length);
                segments.Add(data[start..end]);
            }
            
            // Build the merkle tree leaves.
            leaves = segments.Select(segment => 
            {
                var leafByteArray = ChunkSegmentToLeafByteArray(segment);
                if (merkleTreeNodesPool.TryDequeue(out var merkleTreeNode))
                    merkleTreeNode.Hash = leafByteArray;
                else
                    merkleTreeNode = new MerkleTreeNode(leafByteArray);
                
                return merkleTreeNode;
            }).ToList();

            //add missing empty leaves
            while (leaves.Count < SegmentsCount)
            {
                if (merkleTreeNodesPool.TryDequeue(out var merkleTreeNode))
                    merkleTreeNode.Hash = new byte[SegmentSize];
                else
                    merkleTreeNode = new MerkleTreeNode(new byte[SegmentSize]);
                
                leaves.Add(merkleTreeNode);
            }
            
            // Build layers and hash.
            layers = [leaves];
            var layerNodes = leaves;
            while (layerNodes.Count > 1)
            {
                var layerIndex = layers.Count;
                layers.Insert(layerIndex, new List<MerkleTreeNode>());
                for (var i = 0; i < layerNodes.Count; i += 2)
                {
                    if (i + 1 == layerNodes.Count &&
                        layerNodes.Count % 2 == 1)
                    {
                        layers[layerIndex].Add(layerNodes[i].Clone());
                        continue;
                    }
                    
                    var left = layerNodes[i];
                    var right = i + 1 == layerNodes.Count ? left : layerNodes[i + 1];
                    var hash = ConcatAndHashPair(left.Hash, right.Hash, hasher);

                    if (merkleTreeNodesPool.TryDequeue(out var merkleTreeNode))
                        merkleTreeNode.Hash = hash;
                    else
                        merkleTreeNode = new MerkleTreeNode(hash);
                    
                    layers[layerIndex].Add(merkleTreeNode);
                }
                layerNodes = layers[layerIndex];
            }
            root =  layerNodes[0];
            
            return hasher.ComputeHash(span.Concat(root.Hash).ToArray());
        }

        public bool VerifyProof(IEnumerable<byte[]> proof, byte[] chunkSegment)
        {
            if (Root is null)
                throw new InvalidOperationException("Hash hasn't been calculated");
            
            return VerifyProof(proof, Root.Hash, hasher.ComputeHash(ChunkSegmentToLeafByteArray(chunkSegment)), hasher);
        }

        // Public static methods.
        public static byte[] ConcatAndHashPair(byte[] left, byte[] right, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            return hasher.ComputeHash(left.Concat(right).ToArray());
        }

        public static bool VerifyProof(
            IEnumerable<byte[]> proof,
            byte[] rootHash,
            byte[] itemHash,
            IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            var hash = itemHash;
            foreach (var proofHash in proof)
                hash = ConcatAndHashPair(proofHash, hash, hasher);
            
            return hash.SequenceEqual(rootHash);
        }
        
        // Helpers.
        /// <summary>
        /// Verify that chunk segment data has right size
        /// </summary>
        /// <param name="data">Input raw data</param>
        /// <returns>Leaf data</returns>
        private static byte[] ChunkSegmentToLeafByteArray(byte[] data)
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
}