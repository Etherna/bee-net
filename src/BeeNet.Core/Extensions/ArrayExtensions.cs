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
    public static class ArrayExtensions
    {
        public static DateTimeOffset NanosecondsUnixTimeToDateTimeOffset(this byte[] unixTimeBytes) =>
            NanosecondsUnixTimeToDateTimeOffset((ReadOnlySpan<byte>)unixTimeBytes.AsSpan());
        
        public static DateTimeOffset NanosecondsUnixTimeToDateTimeOffset(this Span<byte> unixTimeBytes) =>
            NanosecondsUnixTimeToDateTimeOffset((ReadOnlySpan<byte>)unixTimeBytes);
        
        public static DateTimeOffset NanosecondsUnixTimeToDateTimeOffset(this ReadOnlySpan<byte> unixTimeBytes)
        {
            if (unixTimeBytes.Length != SwarmFeedChunk.TimeStampSize)
                throw new ArgumentOutOfRangeException(nameof(unixTimeBytes), "Invalid unix time byte array length");
            
            var unixNanoseconds = BinaryPrimitives.ReadUInt64BigEndian(unixTimeBytes);
            var unixMilliseconds = unixNanoseconds / 1000000;
            return DateTimeOffset.FromUnixTimeMilliseconds((long)unixMilliseconds);
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
