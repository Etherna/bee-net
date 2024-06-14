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

namespace Etherna.BeeNet.Hasher.Store
{
    public class StampStoreItem(
        PostageBatchId batchId,
        SwarmHash chunkHash)
    {
        // Consts.
        public const string NamespaceStr = "stampItem";
        
        // Properties.
        public PostageBatchId BatchId { get; protected set; } = batchId;
        public DateTimeOffset? BucketTimestamp { get; set; }
        public SwarmHash ChunkHash { get; protected set; } = chunkHash;
        public string Id => string.Join("/", BatchId.ToString(), ChunkHash.ToString());
        public StampBucketIndex? StampBucketIndex { get; set; }
        public string StoreKey => string.Join("/", NamespaceStr, Id);
    }
}