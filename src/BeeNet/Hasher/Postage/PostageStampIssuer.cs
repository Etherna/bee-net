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

namespace Etherna.BeeNet.Hasher.Postage
{
    public class PostageStampIssuer : IPostageStampIssuer
    {
        // Fields.
        private readonly uint[] _buckets = new uint[1 << PostageBatch.BucketDepth];
        
        // Constructor.
        public PostageStampIssuer(
            PostageBatch postageBatch)
        {
            ArgumentNullException.ThrowIfNull(postageBatch, nameof(postageBatch));
            
            BucketUpperBound = (uint)1 << (postageBatch.Depth - PostageBatch.BucketDepth);
            PostageBatch = postageBatch;
        }
        
        // Properties.
        public ReadOnlySpan<uint> Buckets => _buckets;
        public uint BucketUpperBound { get; }
        public bool HasSaturated { get; private set; }
        public PostageBatch PostageBatch { get; }
        public uint MaxBucketCount { get; private set; }

        // Methods.
        public StampBucketIndex IncrementBucketCount(SwarmAddress address)
        {
            var bucketId = address.ToBucketId();

            if (_buckets[bucketId] == BucketUpperBound)
            {
                if (PostageBatch.IsImmutable)
                    throw new InvalidOperationException("Immutable postage overflowed");
                HasSaturated = true;
                _buckets[bucketId] = 0;
            }

            _buckets[bucketId]++;
            if (_buckets[bucketId] > MaxBucketCount)
                MaxBucketCount = _buckets[bucketId];

            return new StampBucketIndex(bucketId, _buckets[bucketId]);
        }
    }
}