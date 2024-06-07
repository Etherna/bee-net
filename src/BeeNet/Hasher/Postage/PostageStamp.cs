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
    internal class PostageStamp
    {
        // Constructor.
        public PostageStamp(
            PostageBatchId batchId,
            StampBucketIndex stampBucketIndex,
            DateTimeOffset timeStamp,
            byte[] signature)
        {
            ArgumentNullException.ThrowIfNull(batchId, nameof(batchId));
            
            BatchId = batchId;
            StampBucketIndex = stampBucketIndex;
            TimeStamp = timeStamp;
            Signature = signature;
        }

        /// <summary>
        /// Postage batch ID
        /// </summary>
        public PostageBatchId BatchId { get; }

        /// <summary>
        /// Index of the batch
        /// </summary>
        public StampBucketIndex StampBucketIndex { get; }

        /// <summary>
        /// To signal order when assigning the indexes to multiple chunks
        /// </summary>
        public DateTimeOffset TimeStamp { get; }

        /// <summary>
        /// common r[32]s[32]v[1]-style 65 byte ECDSA signature of batchID|index|address by owner or grantee
        /// </summary>
        public ReadOnlyMemory<byte> Signature { get; }
    }
}