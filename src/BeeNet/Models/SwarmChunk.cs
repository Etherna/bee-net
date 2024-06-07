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

using Etherna.BeeNet.Hasher.Bmt;
using System;
using System.Buffers.Binary;

namespace Etherna.BeeNet.Models
{
    public class SwarmChunk
    {
        // Fields.
        protected readonly byte[] _data;
        protected readonly byte[] _span;
        
        // Consts.
        public const int DataSize = SwarmChunkBmt.SegmentSize * SwarmChunkBmt.SegmentsCount;
        public const int SpanAndDataSize = SpanSize + DataSize;
        public const int SpanSize = 8;
        
        // Constructors.
        public SwarmChunk(SwarmAddress address, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
            
            Address = address;
            _span = LengthToSpan((ulong)data.Length);
            _data = data;
        }

        internal SwarmChunk(SwarmAddress address, byte[] span, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            Address = address;
            _span = span;
            _data = data;
        }
        
        // Static builders.
        internal static SwarmChunk BuildFromSpanAndData(SwarmAddress address, byte[] spanAndData)
        {
            if (spanAndData.Length > SpanAndDataSize)
                throw new ArgumentOutOfRangeException(nameof(spanAndData),
                    $"Data with span can't be longer than {SpanAndDataSize} bytes");

            var spanSlice = spanAndData[..SpanSize];
            var dataSlice = spanAndData[SpanSize..];

            return new SwarmChunk(address, spanSlice, dataSlice);
        }

        // Properties.
        public SwarmAddress Address { get; }
        public ReadOnlyMemory<byte> Data => _data;
        public ReadOnlyMemory<byte> Span => _span;
        
        // Static methods.
        public static byte[] LengthToSpan(ulong length)
        {
            var span = new byte[SpanSize];
            WriteSpan(span, length);
            return span;
        }

        public static ulong SpanToLength(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadUInt64LittleEndian(span);

        public static void WriteSpan(Span<byte> span, ulong length) =>
            BinaryPrimitives.WriteUInt64LittleEndian(span, length);
    }
}