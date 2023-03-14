namespace Etherna.BeeNet.Feeds.Models
{
    public abstract class FeedIndexBase
    {
        // Properties.
        public abstract byte[] MarshalBinary { get; }

        // Methods.
        public abstract FeedIndexBase GetNext(ulong at);
    }
}