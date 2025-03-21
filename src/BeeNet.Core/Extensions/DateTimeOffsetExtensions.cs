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

namespace Etherna.BeeNet.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static byte[] ToUnixTimeNanosecondsByteArray(this DateTimeOffset dateTime)
        {
            var unixMilliseconds = (ulong)dateTime.ToUnixTimeMilliseconds();
            var unixNanoseconds = unixMilliseconds * 1000000;
            
            var unixTimeByteArray = new byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(unixTimeByteArray, unixNanoseconds);
            return unixTimeByteArray;
        }
    }
}