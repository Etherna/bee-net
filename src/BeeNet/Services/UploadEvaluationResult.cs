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

using Etherna.BeeNet.Hasher.Postage;
using Etherna.BeeNet.Models;
using System;

namespace Etherna.BeeNet.Services
{
    public class UploadEvaluationResult
    {
        // Constructor.
        internal UploadEvaluationResult(
            SwarmAddress address,
            IPostageStampIssuer postageStampIssuer)
        {
            Address = address;
            PostageStampIssuer = postageStampIssuer;
        }

        // Properties.
        /// <summary>
        /// The upload resulting address
        /// </summary>
        public SwarmAddress Address { get; }

        public IPostageStampIssuer PostageStampIssuer { get; }

        /// <summary>
        /// Total batch space consumed in bytes
        /// </summary>
        public long ConsumedSize =>
            PostageStampIssuer.MaxBucketCount *
            (long)Math.Pow(2, PostageBatch.BucketDepth) *
            SwarmChunk.DataSize;
        
        /// <summary>
        /// Available postage batch space after the upload, with minimum batch depth
        /// </summary>
        public long RemainingPostageBatchSize => RequiredPostageBatchByteSize - ConsumedSize;

        /// <summary>
        /// Minimum required postage batch depth to handle the upload
        /// </summary>
        public int RequiredPostageBatchDepth =>
            Math.Max(
                (int)Math.Ceiling(Math.Log2(PostageStampIssuer.MaxBucketCount)) + PostageBatch.BucketDepth,
                PostageBatch.MinDepth);
        
        /// <summary>
        /// Minimum required postage batch byte size
        /// </summary>
        public long RequiredPostageBatchByteSize =>
            (long)(Math.Pow(2, RequiredPostageBatchDepth) * SwarmChunk.DataSize);
    }
}