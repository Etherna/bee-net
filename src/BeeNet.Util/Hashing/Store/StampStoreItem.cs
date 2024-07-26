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

namespace Etherna.BeeNet.Hashing.Store
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