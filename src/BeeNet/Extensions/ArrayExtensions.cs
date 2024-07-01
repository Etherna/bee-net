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
        public static ulong ByteArrayToUnixDateTime(this byte[] dateTimeByteArray)
        {
            ArgumentNullException.ThrowIfNull(dateTimeByteArray, nameof(dateTimeByteArray));

            if (dateTimeByteArray.Length != SwarmFeedChunk.TimeStampSize)
                throw new ArgumentOutOfRangeException(nameof(dateTimeByteArray), "Invalid date time byte array length");

            return BinaryPrimitives.ReadUInt64BigEndian(dateTimeByteArray);
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
