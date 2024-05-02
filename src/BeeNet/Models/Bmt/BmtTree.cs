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

namespace Etherna.BeeNet.Models.Bmt
{
    /// <summary>
    /// TODO: Try to replace with MerkleTree from Nethereum.Merkle
    /// </summary>
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class BmtTree
    {
        // Constructor.
        public BmtTree(int segmentSize, int maxSize, int depth, Func<byte[], byte[]> hasher)
        {
            throw new System.NotImplementedException();
        }
        
        // Properties.
        /// <summary>
        /// Leaf nodes of the tree, other nodes accessible via parent links
        /// </summary>
        public BmtTreeNode[] Leaves { get; }
        
        public byte[] Buffer { get; }
    }
}