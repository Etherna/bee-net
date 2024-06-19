using Etherna.BeeNet.Models;
using System;

namespace Etherna.BeeNet.Hasher.Postage
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