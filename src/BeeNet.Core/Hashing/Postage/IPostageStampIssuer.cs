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

namespace Etherna.BeeNet.Hashing.Postage
{
    public interface IPostageStampIssuer
    {
        // Properties.
        IReadOnlyPostageBuckets Buckets { get; }
        
        uint BucketUpperBound { get; }
        
        /// <summary>
        /// True if batch is mutable and BucketUpperBound has been it
        /// </summary>
        bool HasSaturated { get; }

        /// <summary>
        /// The batch stamps are issued from
        /// </summary>
        PostageBatch PostageBatch { get; }
        
        EthAddress? PostageBatchOwner { get; }

        // Methods.
        StampBucketIndex IncrementBucketCount(SwarmHash hash);
    }
}