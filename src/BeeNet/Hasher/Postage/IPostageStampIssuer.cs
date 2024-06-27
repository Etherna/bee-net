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

namespace Etherna.BeeNet.Hasher.Postage
{
    public interface IPostageStampIssuer
    {
        // Properties.
        /// <summary>
        /// Collision Buckets: counts per neighbourhoods
        /// </summary>
        public ReadOnlySpan<uint> Buckets { get; }

        public uint BucketUpperBound { get; }
        
        /// <summary>
        /// True if batch is mutable and BucketUpperBound has been it
        /// </summary>
        public bool HasSaturated { get; }

        /// <summary>
        /// The batch stamps are issued from
        /// </summary>
        public PostageBatch PostageBatch { get; }
        
        /// <summary>
        /// The count of the fullest bucket
        /// </summary>
        public uint MaxBucketCount { get; }

        long TotalChunks { get; }

        // Methods.
        StampBucketIndex IncrementBucketCount(SwarmHash hash);
        
        ulong GetCollisions(uint bucketId);
    }
}