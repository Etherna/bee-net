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

using System;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Merkle
{
    /// <summary>
    /// TODO: Try to replace with MerkleTree from Nethereum.Merkle
    /// </summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class Bmt
    {
        // Fields.
        private readonly byte[] _buffer;
        
        // Constructor.
        public Bmt(int maxSize, int depth, Func<byte[], byte[]> hashFunc)
        {
            var n = new BmtNode(0, null, hashFunc);
            var prevLevel = new[] { n };
            
            // iterate over levels and creates 2^(depth-level) nodes
            // the 0 level is on double segment sections so we start at depth - 2
            var count = 2;
            for (var level = depth - 2; level >= 0; level--)
            {
                var nodes = new BmtNode[count];
                for (var i = 0; i < count; i++)
                {
                    var parent = prevLevel[i / 2];
                    nodes[i] = new BmtNode(i, parent, hashFunc);
                }

                prevLevel = nodes;
                count *= 2;
            }
            
            // the datanode level is the nodes on the last level
            _buffer = new byte[maxSize];
            Leaves = prevLevel;
        }
        
        // Properties.
        public Span<byte> Buffer => _buffer;
        
        /// <summary>
        /// Leaf nodes of the tree, other nodes accessible via parent links
        /// </summary>
        public BmtNode[] Leaves { get; }
    }
}