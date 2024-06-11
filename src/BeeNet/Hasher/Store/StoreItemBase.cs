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
    public abstract class StoreItemBase
    {
        // Constructor.
        protected StoreItemBase(
            PostageBatchId batchId,
            SwarmHash chunkHash)
        {
            BatchId = batchId;
            ChunkHash = chunkHash;
        }

        // Properties.
        public PostageBatchId BatchId { get; protected set; }
        public DateTimeOffset? BucketTimestamp { get; set; }
        public SwarmHash ChunkHash { get; protected set; }
        
        /// <summary>
        /// ID is the unique identifier of Item.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Namespace is used to separate similar items.
        /// E.g.: can be seen as a table construct.
        /// </summary>
        public abstract string NamespaceStr { get; }
        public StampBucketIndex? StampBucketIndex { get; set; }
        public string StoreKey => string.Join("/", NamespaceStr, Id);
    }
}