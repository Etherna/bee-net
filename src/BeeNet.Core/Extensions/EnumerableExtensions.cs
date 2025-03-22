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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Extensions
{
    public static class EnumerableExtensions
    {
        public static DateTimeOffset UnixTimeNanosecondsToDateTimeOffset(this byte[] unixTimeBytes) =>
            UnixTimeNanosecondsToDateTimeOffset((ReadOnlySpan<byte>)unixTimeBytes.AsSpan());
        
        public static DateTimeOffset UnixTimeNanosecondsToDateTimeOffset(this Span<byte> unixTimeBytes) =>
            UnixTimeNanosecondsToDateTimeOffset((ReadOnlySpan<byte>)unixTimeBytes);
        
        public static DateTimeOffset UnixTimeNanosecondsToDateTimeOffset(this ReadOnlySpan<byte> unixTimeBytes)
        {
            if (unixTimeBytes.Length != sizeof(ulong))
                throw new ArgumentOutOfRangeException(nameof(unixTimeBytes), "Invalid unix time byte array length");
            
            var unixNanoseconds = BinaryPrimitives.ReadUInt64BigEndian(unixTimeBytes);
            var unixMilliseconds = unixNanoseconds / 1000000;
            return DateTimeOffset.FromUnixTimeMilliseconds((long)unixMilliseconds);
        }
        
        public static DateTimeOffset UnixTimeSecondsToDateTimeOffset(this byte[] unixTimeBytes) =>
            UnixTimeSecondsToDateTimeOffset((ReadOnlySpan<byte>)unixTimeBytes.AsSpan());
        
        public static DateTimeOffset UnixTimeSecondsToDateTimeOffset(this Span<byte> unixTimeBytes) =>
            UnixTimeSecondsToDateTimeOffset((ReadOnlySpan<byte>)unixTimeBytes);
        
        public static DateTimeOffset UnixTimeSecondsToDateTimeOffset(this ReadOnlySpan<byte> unixTimeBytes)
        {
            if (unixTimeBytes.Length != sizeof(ulong))
                throw new ArgumentOutOfRangeException(nameof(unixTimeBytes), "Invalid unix time byte array length");
            
            var unixSeconds = BinaryPrimitives.ReadUInt64BigEndian(unixTimeBytes);
            return DateTimeOffset.FromUnixTimeSeconds((long)unixSeconds);
        }

        public static string FindCommonPrefix(this string x, string y)
        {
            ArgumentNullException.ThrowIfNull(x, nameof(x));
            ArgumentNullException.ThrowIfNull(y, nameof(y));
            return new(FindCommonPrefix(x.ToCharArray(), y.ToCharArray()));
        }

        public static T[] FindCommonPrefix<T>(this T[] x, T[] y) =>
            x.TakeWhile((current, i) => i < y.Length && EqualityComparer<T>.Default.Equals(y[i], current)).ToArray();
    }
}
