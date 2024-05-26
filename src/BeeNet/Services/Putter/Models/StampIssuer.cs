// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Models;
using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Services.Putter.Models
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class StampIssuer
    {
        // Consts.
        public const int BucketDepth = 16;
        public const int IndexSize = 8;
        public const int StampSize = 113;
        
        // Constructor.
        public StampIssuer(byte[] batchId)
        {
            BatchID = batchId;
            Buckets = Array.Empty<uint>();
        }
        
        // Properties.
        /// <summary>
        /// Label to identify the batch period/importance
        /// </summary>
        public string? Label { get; set; }
        
        /// <summary>
        /// Owner identity
        /// </summary>
        /// <returns></returns>
        public string? KeyID { get; set; }
        
        /// <summary>
        /// The batch stamps are issued from
        /// </summary>
        public byte[] BatchID { get; set; }
        
        /// <summary>
        /// Amount paid for the batch
        /// </summary>
        /// <returns></returns>
        public long BatchAmount { get; set; }
        
        /// <summary>
        /// Batch depth: batch size = 2^{depth}
        /// </summary>
        /// <returns></returns>
        public byte BatchDepth { get; set; }
        
        /// <summary>
        /// Collision Buckets: counts per neighbourhoods (limited to 2^{batchdepth-bucketdepth})
        /// </summary>
        public uint[] Buckets { get; set; }

        public uint BucketUpperBound => (uint)1 << (BatchDepth - BucketDepth);
        
        /// <summary>
        /// the count of the fullest bucket
        /// </summary>
        /// <returns></returns>
        public uint MaxBucketCount { get; set; }
        
        /// <summary>
        /// BlockNumber when this batch was created
        /// </summary>
        /// <returns></returns>
        public ulong BlockNumber { get; set; }
        
        /// <summary>
        /// Specifies immutability of the created batch
        /// </summary>
        /// <returns></returns>
        public bool ImmutableFlag { get; set; }

        // Methods.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns>BatchIndex</returns>
        public byte[] Increment(SwarmAddress address)
        {
            var bIdx = ToBucket(address);
            var bCnt = Buckets[bIdx];

            if (bCnt == BucketUpperBound)
            {
                if (ImmutableFlag)
                    throw new InvalidOperationException("Immutable postage overflowed");

                bCnt = 0;
                Buckets[bIdx] = 0;
            }

            Buckets[bIdx]++;
            if (Buckets[bIdx] > MaxBucketCount)
                MaxBucketCount = Buckets[bIdx];

            return IndexToBytes(bIdx, bCnt);
        }

        // Helpers.
        /// <summary>
        /// Creates an uint64 index from
        /// - bucket index (neighbourhood index, 2^depth, bytes 2-4)
        /// - and the within-bucket index, 2^(batchdepth-bucketdepth), bytes 5-8)
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="index"></param>
        private static byte[] IndexToBytes(uint bucket, uint index)
        {
            var buf = new byte[IndexSize];
            BinaryPrimitives.WriteUInt32BigEndian(buf, bucket);
            BinaryPrimitives.WriteUInt32BigEndian(buf.AsSpan()[4..], index);
            return buf;
        }
        
        private static uint ToBucket(SwarmAddress address)
        {
            byte[] firstFourBytes = address.ToByteArray()[..4];
            if (BitConverter.IsLittleEndian)
                Array.Reverse(firstFourBytes);
            var result = BitConverter.ToUInt32(firstFourBytes, 0);
            return result >> (32 - BucketDepth);
        }
    }
}