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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmChunkBmt
    {
        // Consts.
        public const int SegmentsCount = SwarmCac.DataSize / SegmentSize;
        public const int SegmentSize = SwarmHash.HashSize;
        
        // Fields.
        private List<List<SwarmChunkBmtNode>> layers = [];
        private List<SwarmChunkBmtNode> leaves = [];
        private readonly Queue<SwarmChunkBmtNode> merkleTreeNodesPool = new();
        private SwarmChunkBmtNode? root;

        // Constructor.
        public SwarmChunkBmt(Hasher? hasher = null)
        {
            /*
             * It's acceptable to instantiate a new hasher here by default, because often ChunkBmt
             * is already cached by itself, and in that case it will need its own hasher instance.
             * In cases where hasher needs to be reused with a throw-away chunk bmt instance,
             * it can be passed as argument.
             */
            Hasher = hasher ?? new Hasher();
        }

        // Properties.
        public Hasher Hasher { get; }
        public SwarmChunkBmtNode? Root => root;

        // Methods.
        public void Clear()
        {
            foreach (var merkleTreeNode in layers.SelectMany(l => l))
                merkleTreeNodesPool.Enqueue(merkleTreeNode);
            layers.Clear();
            leaves.Clear();
            root = null;
        }
        
        public IReadOnlyCollection<ReadOnlyMemory<byte>> GetProof(ReadOnlyMemory<byte> chunkSegment)
        {
            var leafByteArray = new byte[SegmentSize];
            ChunkSegmentToLeafByteArray(chunkSegment, leafByteArray);
            var hashLeaf = Hasher.ComputeHash(leafByteArray);

            for (var i = 0; i < leaves.Count; i++)
                if (leaves[i].Matches(hashLeaf))
                    return GetProof(i);

            throw new KeyNotFoundException("Leaf not found");
        }

        public IReadOnlyCollection<ReadOnlyMemory<byte>> GetProof(int index)
        {
            var proofs = new List<ReadOnlyMemory<byte>>();
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

        public SwarmHash Hash(ReadOnlyMemory<byte> span, ReadOnlyMemory<byte> data)
        {
            if (data.Length > SwarmCac.DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Max writable data is {SwarmCac.DataSize} bytes");
            
            // Split input data into leaf segments.
            var segments = new List<ReadOnlyMemory<byte>>();
            for (var start = 0; start < data.Length; start += SegmentSize)
            {
                var end = Math.Min(start + SegmentSize, data.Length);
                segments.Add(data[start..end]);
            }
            
            // Build the merkle tree leaves.
            leaves = segments.Select(segment => 
            {
                if (merkleTreeNodesPool.TryDequeue(out var merkleTreeNode))
                    ChunkSegmentToLeafByteArray(segment, merkleTreeNode.Hash);
                else
                {
                    var leafByteArray = new byte[SegmentSize];
                    ChunkSegmentToLeafByteArray(segment, leafByteArray);
                    merkleTreeNode = new SwarmChunkBmtNode(leafByteArray);
                }
                
                return merkleTreeNode;
            }).ToList();

            //add missing empty leaves
            while (leaves.Count < SegmentsCount)
            {
                if (merkleTreeNodesPool.TryDequeue(out var merkleTreeNode))
                    merkleTreeNode.Hash = new byte[SegmentSize];
                else
                    merkleTreeNode = new SwarmChunkBmtNode(new byte[SegmentSize]);
                
                leaves.Add(merkleTreeNode);
            }
            
            // Build layers and hash.
            layers = [leaves];
            var layerNodes = leaves;
            while (layerNodes.Count > 1)
            {
                var layerIndex = layers.Count;
                layers.Insert(layerIndex, new List<SwarmChunkBmtNode>());
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

                    if (merkleTreeNodesPool.TryDequeue(out var merkleTreeNode))
                        Hasher.ComputeHash([left.Hash, right.Hash], merkleTreeNode.Hash.Span);
                    else
                        merkleTreeNode = new SwarmChunkBmtNode(Hasher.ComputeHash([left.Hash, right.Hash]));
                    
                    layers[layerIndex].Add(merkleTreeNode);
                }
                layerNodes = layers[layerIndex];
            }
            root =  layerNodes[0];
            
            return Hasher.ComputeHash([span, root.Hash]);
        }

        public bool VerifyProof(IEnumerable<ReadOnlyMemory<byte>> proof, ReadOnlyMemory<byte> chunkSegment)
        {
            if (Root is null)
                throw new InvalidOperationException("Hash hasn't been calculated");

            var leafData = new byte[SegmentSize];
            ChunkSegmentToLeafByteArray(chunkSegment, leafData);
            return VerifyProof(
                proof,
                Root.Hash,
                Hasher.ComputeHash(leafData),
                Hasher);
        }

        // Public static methods.
        public static bool VerifyProof(
            IEnumerable<ReadOnlyMemory<byte>> proof,
            ReadOnlyMemory<byte> rootHash,
            ReadOnlyMemory<byte> itemHash,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            var hash = itemHash;
            foreach (var proofHash in proof)
                hash = hasher.ComputeHash([proofHash, hash]);
            
            return hash.Span.SequenceEqual(rootHash.Span);
        }
        
        // Helpers.
        /// <summary>
        /// Verify that chunk segment data has right size
        /// </summary>
        /// <param name="data">Input raw data</param>
        /// <param name="outputLeafData">Output leaf formatted data</param>
        private static void ChunkSegmentToLeafByteArray(ReadOnlyMemory<byte> data, Memory<byte> outputLeafData)
        {
            if (outputLeafData.Length != SegmentSize)
                throw new ArgumentOutOfRangeException(nameof(outputLeafData), $"Output length must be {SegmentSize}");
            
            if (data.Length > SegmentSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {SegmentSize}");
            
            outputLeafData.Span.Clear();
            data.CopyTo(outputLeafData);
        }
    }
}