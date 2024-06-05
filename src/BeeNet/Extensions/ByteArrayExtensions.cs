﻿//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.BeeNet.Models;
using System;
using System.Buffers.Binary;
using System.Linq;

namespace Etherna.BeeNet.Extensions
{
    public static class ByteArrayExtensions
    {
        public static ulong ByteArrayToUnixDateTime(this byte[] dateTimeByteArray)
        {
            ArgumentNullException.ThrowIfNull(dateTimeByteArray, nameof(dateTimeByteArray));

            if (dateTimeByteArray.Length != SwarmFeedChunk.TimeStampSize)
                throw new ArgumentOutOfRangeException(nameof(dateTimeByteArray), "Invalid date time byte array length");

            return BinaryPrimitives.ReadUInt64BigEndian(dateTimeByteArray);
        }

        public static byte[] FindCommonPrefixWith(this byte[] x, byte[] y) =>
            x.TakeWhile((current, i) => i < y.Length && y[i] == current).ToArray();
    }
}
