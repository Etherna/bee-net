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
using System.Buffers.Binary;
using System.Linq;

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
        
        // Constructor.
        public SwarmChunk(SwarmAddress address, byte[] data, bool dataHasSpan)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            Address = address;
            if (dataHasSpan)
            {
                if (data.Length > SpanAndDataSize)
                    throw new ArgumentOutOfRangeException(nameof(data), $"Data with span can't be longer than {SpanAndDataSize} bytes");
                
                _span = data[..SpanSize];
                _data = data[SpanSize..];
            }
            else
            {
                if (data.Length > DataSize)
                    throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
                
                _span = LengthToSpan((ulong)data.Length);
                _data = data;
            }
        }
        
        // Properties.
        public SwarmAddress Address { get; }
        public ReadOnlySpan<byte> Data => _data;
        public ReadOnlySpan<byte> Span => _span;
        public ReadOnlySpan<byte> SpanData => _span.Concat(_data).ToArray();
        
        // Internal properties.
        internal PostageStamp? PostageStamp { get; set; }
        
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