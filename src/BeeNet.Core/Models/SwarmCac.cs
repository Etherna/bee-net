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
    public sealed class SwarmCac : SwarmChunk
    {
        // Consts.
        public const int DataSize = 4096;
        public const int SpanDataSize = SpanSize + DataSize;
        public const int SpanSize = 8;
        
        // Constructors.
        public SwarmCac(SwarmHash hash, ReadOnlyMemory<byte> spanData)
        {
            if (spanData.Length < SpanSize)
                throw new ArgumentOutOfRangeException(nameof(spanData),
                    $"Data with span can't be shorter than {SpanSize} bytes");
            if (spanData.Length > SpanDataSize)
                throw new ArgumentOutOfRangeException(nameof(spanData),
                    $"Data with span can't be longer than {SpanDataSize} bytes");
            
            Hash = hash;
            SpanData = spanData;
        }
        
        public SwarmCac(SwarmHash hash, ReadOnlyMemory<byte> span, ReadOnlyMemory<byte> data)
        {
            if (span.Length != SpanSize)
                throw new ArgumentOutOfRangeException(nameof(span), $"Span size must be {SpanSize} bytes");
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
            
            Hash = hash;
            
            var spanDataBuffer = new byte[SpanSize + data.Length];
            span.CopyTo(spanDataBuffer);
            data.CopyTo(spanDataBuffer.AsMemory(SpanSize));
            SpanData = spanDataBuffer;
        }
        
        // Static builders.
        public static SwarmCac BuildFromData(SwarmHash hash, ReadOnlyMemory<byte> data)
        {
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
            
            var spanDataBuffer = new byte[SpanSize + data.Length];
            LengthToSpan((ulong)data.Length).CopyTo(spanDataBuffer, 0);
            data.CopyTo(spanDataBuffer.AsMemory(SpanSize));

            return new(hash, spanDataBuffer);
        }

        // Properties.
        public override SwarmHash Hash { get; }
        public ReadOnlyMemory<byte> Span => SpanData[..SpanSize];
        public ReadOnlyMemory<byte> Data => SpanData[SpanSize..];
        public ReadOnlyMemory<byte> SpanData { get; }
        
        // Methods.
        public override ReadOnlyMemory<byte> GetFullPayload() => SpanData;
        public override byte[] GetFullPayloadToByteArray() => SpanData.ToArray();

        // Static methods.
        public static bool IsValid(SwarmHash hash, ReadOnlyMemory<byte> spanData, SwarmChunkBmt swarmChunkBmt)
        {
            ArgumentNullException.ThrowIfNull(swarmChunkBmt, nameof(swarmChunkBmt));

            if (spanData.Length is < SpanSize or > SpanDataSize)
                return false;
            return hash == swarmChunkBmt.Hash(spanData);
        }
        
        public static byte[] LengthToSpan(ulong length)
        {
            var span = new byte[SpanSize];
            WriteSpan(length, span);
            return span;
        }

        public static ulong SpanToLength(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadUInt64LittleEndian(span);

        public static void WriteSpan(ulong length, Span<byte> outputSpan) =>
            BinaryPrimitives.WriteUInt64LittleEndian(outputSpan, length);
    }
}