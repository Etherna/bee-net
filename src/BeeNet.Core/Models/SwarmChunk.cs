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

using System;
using System.Buffers.Binary;

namespace Etherna.BeeNet.Models
{
    public class SwarmChunk
    {
        // Consts.
        public const int DataSize = 4096;
        public const int SpanAndDataSize = SpanSize + DataSize;
        public const int SpanSize = 8;
        
        // Fields.
#pragma warning disable CA1051
        protected readonly byte[] _data;
        protected readonly byte[] _span;
#pragma warning restore CA1051
        
        // Constructors.
        public SwarmChunk(SwarmHash hash, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
            
            Hash = hash;
            _span = LengthToSpan((ulong)data.Length);
            _data = data;
        }

        public SwarmChunk(SwarmHash hash, byte[] span, byte[] data)
        {
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            if (span.Length != SpanSize)
                throw new ArgumentOutOfRangeException(nameof(span), $"Span size must be {SpanSize} bytes");
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
            
            Hash = hash;
            _span = span;
            _data = data;
        }
        
        // Static builders.
        public static SwarmChunk BuildFromSpanAndData(SwarmHash hash, ReadOnlySpan<byte> spanAndData)
        {
            if (spanAndData.Length > SpanAndDataSize)
                throw new ArgumentOutOfRangeException(nameof(spanAndData),
                    $"Data with span can't be longer than {SpanAndDataSize} bytes");

            var spanSlice = spanAndData[..SpanSize];
            var dataSlice = spanAndData[SpanSize..];

            return new SwarmChunk(hash, spanSlice.ToArray(), dataSlice.ToArray());
        }

        // Properties.
        public SwarmHash Hash { get; }
        public ReadOnlyMemory<byte> Data => _data;
        public ReadOnlyMemory<byte> Span => _span;
        
        // Methods.
        public byte[] GetSpanAndData()
        {
            var spanAndData = new byte[SpanSize + _data.Length];
            Span.CopyTo(spanAndData.AsMemory()[..SpanSize]);
            Data.CopyTo(spanAndData.AsMemory()[SpanSize..]);
            return spanAndData;
        }
        
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