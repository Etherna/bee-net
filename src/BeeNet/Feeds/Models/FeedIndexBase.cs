using System;

namespace Etherna.BeeNet.Feeds.Models
{
    public abstract class FeedIndexBase
    {
        // Properties.
        public abstract byte[] MarshalBinary { get; }

        // Methods.
        public FeedIndexBase GetNext(DateTimeOffset at) =>
            GetNext((ulong)at.ToUnixTimeSeconds());

        public abstract FeedIndexBase GetNext(ulong at);
    }
}