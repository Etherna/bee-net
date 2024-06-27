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