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

namespace Etherna.BeeNet.Hashing.Postage
{
    public class PostageStampIssuer : IPostageStampIssuer
    {
        // Fields.
        private readonly PostageBuckets _buckets;
        
        // Constructor.
        public PostageStampIssuer(
            PostageBatch postageBatch,
            EthAddress? postageBatchOwner = null,
            PostageBuckets? buckets = null)
        {
            ArgumentNullException.ThrowIfNull(postageBatch, nameof(postageBatch));
            if (!postageBatch.Depth.HasValue)
                throw new ArgumentException("Batch depth can't be null");

            _buckets = buckets ?? new PostageBuckets();
            BucketUpperBound = (uint)1 << (postageBatch.Depth.Value - PostageBatch.BucketDepth);
            PostageBatch = postageBatch;
            PostageBatchOwner = postageBatchOwner;
        }

        // Properties.
        public IReadOnlyPostageBuckets Buckets => _buckets;
        public uint BucketUpperBound { get; }
        public bool HasSaturated { get; private set; }
        public PostageBatch PostageBatch { get; }
        public EthAddress? PostageBatchOwner { get; }

        // Methods.
        public PostageBucketIndex IncrementBucketCount(SwarmHash hash)
        {
            var bucketId = hash.ToBucketId();

            var collisions = _buckets.GetCollisions(bucketId);
            if (collisions == BucketUpperBound)
            {
                if (PostageBatch.IsImmutable)
                    throw new InvalidOperationException("Immutable postage overflowed");
                HasSaturated = true;
                _buckets.ResetBucketCollisions(bucketId);
                collisions = 0;
            }

            _buckets.IncrementCollisions(bucketId);
            collisions++;

            return new PostageBucketIndex(bucketId, collisions);
        }
    }
}