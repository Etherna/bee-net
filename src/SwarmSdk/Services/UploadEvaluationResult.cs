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

using Etherna.SwarmSdk.Hashing.Postage;
using Etherna.SwarmSdk.Models;
using System;

namespace Etherna.SwarmSdk.Services
{
    public class UploadEvaluationResult
    {
        // Constructor.
        internal UploadEvaluationResult(
            SwarmReference reference,
            long missedOptimisticHashing,
            IPostageStampIssuer postageStampIssuer)
        {
            Reference = reference;
            MissedOptimisticHashing = missedOptimisticHashing;
            PostageStampIssuer = postageStampIssuer;
        }

        // Properties.
        /// <summary>
        /// The upload resulting reference
        /// </summary>
        public SwarmReference Reference { get; }
        
        public long MissedOptimisticHashing { get; }

        public IPostageStampIssuer PostageStampIssuer { get; }

        /// <summary>
        /// Total batch space consumed in bytes
        /// </summary>
        public long ConsumedSize =>
            PostageStampIssuer.Buckets.MaxBucketCollisions *
            (long)Math.Pow(2, PostageBatch.BucketDepth) *
            SwarmCac.DataSize;
        
        /// <summary>
        /// Available postage batch space after the upload, with minimum batch depth
        /// </summary>
        public long RemainingPostageBatchSize => RequiredPostageBatchByteSize - ConsumedSize;
        
        /// <summary>
        /// Minimum required postage batch byte size
        /// </summary>
        public long RequiredPostageBatchByteSize =>
            (long)(Math.Pow(2, PostageStampIssuer.Buckets.RequiredPostageBatchDepth) * SwarmCac.DataSize);
    }
}