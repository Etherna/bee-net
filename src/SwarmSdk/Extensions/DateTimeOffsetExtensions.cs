// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
// 
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Buffers.Binary;

namespace Etherna.SwarmSdk.Extensions
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
        
        public static byte[] ToUnixTimeSecondsByteArray(this DateTimeOffset dateTime)
        {
            var unixSeconds = (ulong)dateTime.ToUnixTimeSeconds();
            
            var unixTimeByteArray = new byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(unixTimeByteArray, unixSeconds);
            return unixTimeByteArray;
        }
    }
}