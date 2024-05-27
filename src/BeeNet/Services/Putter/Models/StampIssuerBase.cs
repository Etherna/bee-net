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
    public abstract class StampIssuerBase
    {
        // Fields.
        private readonly uint[] _buckets;
        
        // Consts.
        public const int BucketDepth = 16;
        public const int IndexSize = 8;
        public const int StampSize = 113;
        
        // Constructor.
        protected StampIssuerBase(
            string? label,
            string keyId,
            PostageBatchId batchId,
            long batchAmount,
            byte batchDepth,
            byte bucketDepth,
            ulong blockNumber,
            bool immutableFlag)
        {
            BatchAmount = batchAmount;
            BatchId = batchId;
            BatchDepth = batchDepth;
            BlockNumber = blockNumber;
            _buckets = new uint[1 << bucketDepth];
            ImmutableFlag = immutableFlag;
            KeyId = keyId;
            Label = label;
        }
        
        // Properties.
        /// <summary>
        /// Amount paid for the batch
        /// </summary>
        /// <returns></returns>
        public long BatchAmount { get; set; }
        
        /// <summary>
        /// The batch stamps are issued from
        /// </summary>
        public PostageBatchId BatchId { get; }
        
        /// <summary>
        /// Batch depth: batch size = 2^{depth}
        /// </summary>
        /// <returns></returns>
        public byte BatchDepth { get; set; }
        
        /// <summary>
        /// BlockNumber when this batch was created
        /// </summary>
        /// <returns></returns>
        public ulong BlockNumber { get; set; }

        /// <summary>
        /// Collision Buckets: counts per neighbourhoods
        /// </summary>
        public ReadOnlySpan<uint> Buckets => _buckets;

        public uint BucketUpperBound => (uint)1 << (BatchDepth - BucketDepth);
        
        /// <summary>
        /// Specifies immutability of the created batch
        /// </summary>
        /// <returns></returns>
        public bool ImmutableFlag { get; set; }
        
        /// <summary>
        /// Owner identity
        /// </summary>
        /// <returns></returns>
        public string KeyId { get; set; }
        
        /// <summary>
        /// Label to identify the batch period/importance
        /// </summary>
        public string? Label { get; set; }
        
        /// <summary>
        /// the count of the fullest bucket
        /// </summary>
        /// <returns></returns>
        public uint MaxBucketCount { get; set; }

        // Methods.
        /// <summary>
        /// Increment bucket collisions
        /// </summary>
        /// <param name="address"></param>
        /// <returns>BatchIndex</returns>
        public byte[] Increment(SwarmAddress address)
        {
            var bucketIndex = ToBucketIndex(address);
            var bucketCount = _buckets[bucketIndex];

            if (bucketCount == BucketUpperBound)
            {
                if (ImmutableFlag)
                    throw new InvalidOperationException("Immutable postage overflowed");

                bucketCount = 0;
                _buckets[bucketIndex] = 0;
            }

            _buckets[bucketIndex]++;
            if (_buckets[bucketIndex] > MaxBucketCount)
                MaxBucketCount = _buckets[bucketIndex];

            return IndexToBytes(bucketIndex, bucketCount);
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
            var buffer = new byte[IndexSize];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, bucket);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[4..], index);
            return buffer;
        }
        
        private static uint ToBucketIndex(SwarmAddress address)
        {
            var uintBytes = address.ToByteArray()[..sizeof(uint)];
            if (BitConverter.IsLittleEndian)
                Array.Reverse(uintBytes);
            return BitConverter.ToUInt32(uintBytes, 0) >> (sizeof(uint) * 8 - BucketDepth);
        }
    }
}