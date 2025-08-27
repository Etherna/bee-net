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

namespace Etherna.BeeNet.Chunks
{
    public static class ChunkRedundancy
    {
        /// <summary>
        /// Decodes the used redundancy level from span keeping the real byte count for the chunk
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static (RedundancyLevel Level, byte[] DecodedSpan) DecodeSpanLevel(ReadOnlySpan<byte> span)
        {
            if (span.Length != SwarmCac.SpanSize)
                throw new ArgumentException("Span length must be " + SwarmCac.SpanSize);

            var decodedSpan = new byte[SwarmCac.SpanSize];
            span.CopyTo(decodedSpan);
            decodedSpan[SwarmCac.SpanSize - 1] = 0;
            
            if (!IsSpanLevelEncoded(span))
                return (RedundancyLevel.None, decodedSpan);
            
            var level = (RedundancyLevel)(span[SwarmCac.SpanSize - 1] & 127);
            return (level, decodedSpan);
        }

        /// <summary>
        /// Encodes used redundancy level for uploading into span keeping the real byte count for the chunk
        /// </summary>
        /// <param name="span"></param>
        /// <param name="level"></param>
        public static void EncodeSpanLevel(Span<byte> span, RedundancyLevel level)
        {
            if (span.Length != SwarmCac.SpanSize)
                throw new ArgumentException("Span length must be " + SwarmCac.SpanSize);

            // Set parity level in the most significant byte.
            span[SwarmCac.SpanSize - 1] = (byte)(level + 128);
        }

        public static bool IsSpanLevelEncoded(ReadOnlySpan<byte> span)
        {
            if (span.Length != SwarmCac.SpanSize)
                throw new ArgumentException("Span length must be " + SwarmCac.SpanSize);
            
            return span[SwarmCac.SpanSize - 1] > 128;
        }
    }
}