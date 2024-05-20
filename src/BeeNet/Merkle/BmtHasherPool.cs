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
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Etherna.BeeNet.Merkle
{
    /// <summary>
    /// Implement a pool of Merkle Trees, defining a degree of parallelism to calculate hashes
    /// </summary>
    internal class BmtHasherPool : IDisposable
    {
        // Fields.
        private readonly BlockingCollection<Bmt> treeCollection;

        // Constructor.
        public BmtHasherPool(int capacity, Func<byte[], byte[]> hasher, int segmentCount)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            // Define tree depth.
            var actualSegmentCount = 2;
            Depth = 1;
            while (actualSegmentCount < segmentCount)
            {
                Depth++;
                actualSegmentCount *= 2;
            }
            
            // Initialize properties.
            Capacity = capacity;
            HasherFunc = hasher;
            SegmentCount = actualSegmentCount;
            SegmentSize = hasher(Array.Empty<byte>()).Length;
            
            // Initialize the ZeroHashes lookup table.
            ZeroHashes = new byte[Depth + 1][];
            var zerosLayer = new byte[SegmentSize];
            ZeroHashes[0] = zerosLayer;
            for (int i = 1; i < Depth+1; i++)
            {
                zerosLayer = hasher(zerosLayer.Concat(zerosLayer).ToArray());
                ZeroHashes[i] = zerosLayer;
            }
            
            // Initialize trees.
            treeCollection = new BlockingCollection<Bmt>(Capacity);
            for (var i = 0; i < Capacity; i++)
                treeCollection.Add(new Bmt(MaxSize, Depth, hasher));
        }

        // Dispose.
        public void Dispose()
        {
            treeCollection.Dispose();
        }
        
        // Properties.
        /// <summary>
        /// Pool capacity, controls concurrency
        /// </summary>
        public int Capacity { get; }
        
        /// <summary>
        /// Depth of the bmt trees = (int)(log2(segmentCount))+1
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Hashing function
        /// </summary>
        public Func<byte[],byte[]> HasherFunc { get; }

        /// <summary>
        /// The total length of the data (SegmentCount * SegmentSize)
        /// </summary>
        public int MaxSize => SegmentCount * SegmentSize;
        
        /// <summary>
        /// The number of segments on the base level of the BMT
        /// </summary>
        public int SegmentCount { get; }
        
        /// <summary>
        /// Size of leaf segments, stipulated to be = hash size
        /// </summary>
        public int SegmentSize { get; }
        
        /// <summary>
        /// Lookup table for predictable padding subtrees for all levels
        /// TODO: probably it should be moved inside BmtHasher
        /// </summary>
        public byte[][] ZeroHashes { get; }

        // Methods.
        public void Put(BmtHasher bmtHasher)
        {
            ArgumentNullException.ThrowIfNull(bmtHasher, nameof(bmtHasher));
            treeCollection.Add(bmtHasher.Bmt);
        }

        public BmtHasher Get()
        {
            var tree = treeCollection.Take();
            return new BmtHasher(this, tree);
        }
    }
}