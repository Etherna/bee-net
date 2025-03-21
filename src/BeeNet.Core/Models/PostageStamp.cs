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

using Etherna.BeeNet.Extensions;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Etherna.BeeNet.Models
{
    public class PostageStamp(
        PostageBatchId batchId,
        StampBucketIndex stampBucketIndex,
        DateTimeOffset timeStamp,
        byte[] signature)
    {
        // Consts.
        public const int StampSize = 113;
        
        // Static builders.
        public static PostageStamp BuildFromByteArray(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != StampSize)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Invalid stamp length");

            var batchId = new PostageBatchId(bytes[..32].ToArray());
            var stampBucketIndex = StampBucketIndex.BuildFromByteArray(bytes[32..40]);
            var timeStamp = DateTimeOffset.FromUnixTimeSeconds((long)BinaryPrimitives.ReadUInt64BigEndian(bytes[40..48]));
            var signature = bytes[48..].ToArray();

            return new PostageStamp(batchId, stampBucketIndex, timeStamp, signature);
        }
        
        // Properties.
        public PostageBatchId BatchId { get; } = batchId;
        public StampBucketIndex StampBucketIndex { get; } = stampBucketIndex;
        public DateTimeOffset TimeStamp { get; } = timeStamp;
        public ReadOnlyMemory<byte> Signature { get; } = signature;
        
        // Methods.
        public byte[] ToByteArray()
        {
            List<byte> buffer = [];
            buffer.AddRange(BatchId.ToByteArray());
            buffer.AddRange(StampBucketIndex.ToByteArray());
            buffer.AddRange(TimeStamp.ToUnixTimeNanosecondsByteArray());
            buffer.AddRange(Signature.ToArray());
            return buffer.ToArray();
        }
    }
}