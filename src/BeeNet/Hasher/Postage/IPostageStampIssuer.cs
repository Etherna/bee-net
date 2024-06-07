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

        // Methods.
        StampBucketIndex IncrementBucketCount(SwarmAddress address);
    }
}