using Etherna.BeeNet.Models;
using System;

namespace Etherna.BeeNet.Hasher.Postage
{
    public class FakePostageStamper : IPostageStamper
    {
        public PostageStamp Stamp(SwarmHash hash) =>
            new(PostageBatchId.Zero, new StampBucketIndex(0, 0), DateTimeOffset.Now, []);
    }
}