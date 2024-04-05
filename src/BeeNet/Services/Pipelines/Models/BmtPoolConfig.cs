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
using System.Linq;

namespace Etherna.BeeNet.Services.Pipelines.Models
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class BmtPoolConfig
    {
        public BmtPoolConfig(Func<byte[], byte[]> hasher, int segmentCount, int capacity)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));

            var (count, depth) = SizeToParams(segmentCount);
            SegmentSize = hasher(Array.Empty<byte>()).Length;
            Zerohashes = new byte[depth + 1][];
            
            // initialises the zerohashes lookup table
            var zeros = new byte[SegmentSize];
            Zerohashes[0] = zeros;
            for (int i = 1; i < depth+1; i++)
            {
                zeros = hasher(zeros.Concat(zeros).ToArray());
                Zerohashes[i] = zeros;
            }

            Hasher = hasher;
            SegmentCount = segmentCount;
            Capacity = capacity;
            MaxSize = count * SegmentSize;
            Depth = depth;
        }

        /// <summary>
        /// Size of leaf segments, stipulated to be = hash size
        /// </summary>
        public int SegmentSize { get; }
        
        /// <summary>
        /// The number of segments on the base level of the BMT
        /// </summary>
        public int SegmentCount { get; }
        
        /// <summary>
        /// Pool capacity, controls concurrency
        /// </summary>
        public int Capacity { get; }
        
        /// <summary>
        /// Depth of the bmt trees = int(log2(segmentCount))+1
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// The total length of the data (count * size)
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// Lookup table for predictable padding subtrees for all levels
        /// </summary>
        public byte[][] Zerohashes { get; }

        /// <summary>
        /// Base hasher to use for the BMT levels
        /// </summary>
        public Func<byte[], byte[]> Hasher { get; }
        
        // Helpers.
        private static (int count, int depth)  SizeToParams(int n)
        {
            var count = 2;
            var depth = 0;
            while (count < n)
            {
                depth++;
                count *= 2;
            }
            return (count, depth + 1);
        }
    }
}