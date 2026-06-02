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

namespace Etherna.SwarmSdk.Models
{
    public class PostageBucketIndex(ushort bucketId, uint bucketCounter)
    {
        // Consts.
        public const int BucketIndexSize = 8;
        
        // Static builders.
        public static PostageBucketIndex BuildFromByteArray(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != BucketIndexSize)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Invalid bucket index length");

            var bucketId = (ushort)BinaryPrimitives.ReadUInt32BigEndian(bytes);
            var bucketCounter = BinaryPrimitives.ReadUInt32BigEndian(bytes[4..]);

            return new PostageBucketIndex(bucketId, bucketCounter);
        }

        // Properties.
        public uint BucketCounter { get; } = bucketCounter;
        public ushort BucketId { get; } = bucketId;
        
        // Methods.
        public byte[] ToByteArray()
        {
            var buffer = new byte[BucketIndexSize];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, BucketId);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[4..], BucketCounter);
            return buffer;
        }
    }
}