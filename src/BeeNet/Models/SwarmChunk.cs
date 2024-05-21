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

namespace Etherna.BeeNet.Models
{
    public class SwarmChunk
    {
        // Fields.
        private readonly byte[] _data;
        private readonly byte[] _span;
        
        // Consts.
        public const int BmtSegments = 128;
        public const int BmtSegmentSize = 32; //Keccak hash size
        public const int DataSize = BmtSegmentSize * BmtSegments;
        public const int SpanAndDataSize = SpanSize + DataSize;
        public const int SpanSize = 8;
        
        // Constructor.
        public SwarmChunk(SwarmAddress address, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes"); 
            
            Address = address;
            _data = data;
            _span = LengthToSpan((ulong)data.Length);
        }
        
        // Properties.
        public SwarmAddress Address { get; }
        public ReadOnlySpan<byte> Data => _data;
        public ReadOnlySpan<byte> Span => _span;
        
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