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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Hasher.Bmt
{
    internal static class SwarmChunkBmtHasher
    {
        // Static methods.
        public static SwarmHash Hash(byte[] span, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            if (data.Length > SwarmChunkBmt.MaxDataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Max writable data is {SwarmChunkBmt.MaxDataSize} bytes");
            
            // Split input data into leaf segments.
            var segments = new List<byte[]>();
            for (var start = 0; start < data.Length; start += SwarmChunkBmt.SegmentSize)
            {
                var end = Math.Min(start + SwarmChunkBmt.SegmentSize, data.Length);
                segments.Add(data[start..end]);
            }
            
            // Build the merkle tree.
            var bmt = new SwarmChunkBmt();
            bmt.BuildTree(segments);
            var result = bmt.Root.Hash;
            
            return SwarmChunkBmt.ComputeHash(span.Concat(result).ToArray());
        }
    }
}