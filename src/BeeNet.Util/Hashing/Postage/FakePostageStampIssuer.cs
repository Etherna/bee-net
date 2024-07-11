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

namespace Etherna.BeeNet.Hashing.Postage
{
    public class FakePostageStampIssuer : IPostageStampIssuer
    {
        public ReadOnlySpan<uint> Buckets => Array.Empty<uint>();
        public uint BucketUpperBound { get; }
        public bool HasSaturated { get; }
        public PostageBatch PostageBatch => PostageBatch.MaxDepthInstance;
        public uint MaxBucketCount { get; }
        public long TotalChunks { get; }

        public StampBucketIndex IncrementBucketCount(SwarmHash hash) =>
            new StampBucketIndex(0, 0);

        public ulong GetCollisions(uint bucketId) => 0;
    }
}