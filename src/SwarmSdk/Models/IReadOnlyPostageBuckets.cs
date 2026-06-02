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

using System.Collections.Generic;

namespace Etherna.SwarmSdk.Models
{
    public interface IReadOnlyPostageBuckets
    {
        // Properties.
        /// <summary>
        /// The higher level of collisions for a bucket
        /// </summary>
        uint MaxBucketCollisions { get; }
        
        /// <summary>
        /// The lower level of collisions for a bucket
        /// </summary>
        uint MinBucketCollisions { get; }
        
        /// <summary>
        /// Minimum required postage batch depth
        /// </summary>
        int RequiredPostageBatchDepth { get; }
        
        /// <summary>
        /// Total added chunks in buckets
        /// </summary>
        long TotalChunks { get; }
        
        // Methods.
        /// <summary>
        /// Amount of buckets, grouped by collisions
        /// </summary>
        /// <returns>Array with collisions as index, and buckets amount as values</returns>
        int[] CountBucketsByCollisions();
        
        /// <summary>
        /// Get a copy of all buckets
        /// </summary>
        /// <returns>All the buckets</returns>
        uint[] GetBuckets();

        IEnumerable<uint> GetBucketsByCollisions(uint collisions);

        uint GetCollisions(uint bucketId);
    }
}