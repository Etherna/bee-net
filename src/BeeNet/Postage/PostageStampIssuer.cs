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

namespace Etherna.BeeNet.Postage
{
    public class PostageStampIssuer : IPostageStampIssuer
    {
        // Fields.
        private readonly uint[] _buckets;
        
        // Consts.
        public const int BucketIndexSize = 8;
        
        // Constructor.
        public PostageStampIssuer(
            PostageBatch postageBatch,
            string ownerEthAddress)
        {
            _buckets = new uint[1 << PostageBatch.BucketDepth];
            PostageBatch = postageBatch;
            OwnerEthAddress = ownerEthAddress;
        }
        
        // Properties.
        public ReadOnlySpan<uint> Buckets => _buckets;
        public uint BucketUpperBound => (uint)1 << (PostageBatch.Depth - PostageBatch.BucketDepth);
        public bool HasSaturated { get; private set; }
        public PostageBatch PostageBatch { get; }
        public string OwnerEthAddress { get; }
        public uint MaxBucketCount { get; private set; }

        // Methods.
        public byte[] IncrementBucketCount(SwarmAddress address)
        {
            var bucketIndex = address.ToBucketIndex();

            if (_buckets[bucketIndex] == BucketUpperBound)
            {
                if (PostageBatch.IsImmutable)
                    throw new InvalidOperationException("Immutable postage overflowed");
                HasSaturated = true;
                _buckets[bucketIndex] = 0;
            }

            _buckets[bucketIndex]++;
            if (_buckets[bucketIndex] > MaxBucketCount)
                MaxBucketCount = _buckets[bucketIndex];

            return BatchIndexToByteArray(bucketIndex, _buckets[bucketIndex]);
        }

        // Helpers.
        /// <summary>
        /// Creates an uint64 index from
        /// - bucket index (neighbourhood index, 2^depth, bytes 2-4)
        /// - the within-bucket index, 2^(batchdepth-bucketdepth), bytes 5-8)
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="index"></param>
        private static byte[] BatchIndexToByteArray(uint bucket, uint index)
        {
            var buffer = new byte[BucketIndexSize];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, bucket);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[4..], index);
            return buffer;
        }
    }
}