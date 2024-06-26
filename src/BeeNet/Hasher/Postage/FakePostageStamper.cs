using Etherna.BeeNet.Hasher.Signer;
using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System;

namespace Etherna.BeeNet.Hasher.Postage
{
    public class FakePostageStamper : IPostageStamper
    {
        public ISigner Signer { get; } = new FakeSigner();
        public IPostageStampIssuer StampIssuer { get; } = new FakePostageStampIssuer();
        public IStampStore StampStore { get; } = new MemoryStampStore();

        public PostageStamp Stamp(SwarmHash hash) =>
            new(PostageBatchId.Zero, new StampBucketIndex(0, 0), DateTimeOffset.Now, []);
    }
}