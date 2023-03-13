namespace Etherna.BeeNet.Feeds
{
    public interface IFeedIndex
    {
        // Properties.
        byte[] MarshalBinary { get; }

        // Methods.
        IFeedIndex GetNext(ulong at);
    }
}