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

using Etherna.SwarmSdk.Models;
using System;

namespace Etherna.SwarmSdk.Stores
{
    public class StampStoreItem(
        PostageBatchId batchId,
        SwarmHash chunkHash,
        PostageBucketIndex bucketIndex)
    {
        // Properties.
        public PostageBatchId BatchId { get; protected set; } = batchId;
        public DateTimeOffset? BucketTimestamp { get; set; }
        public SwarmHash ChunkHash { get; protected set; } = chunkHash;
        public string Id => BuildId(BatchId, ChunkHash);
        public PostageBucketIndex BucketIndex { get; set; } = bucketIndex;

        // Static methods.
        public static string BuildId(PostageBatchId batchId, SwarmHash chunkHash) => batchId + "/" + chunkHash;
    }
}