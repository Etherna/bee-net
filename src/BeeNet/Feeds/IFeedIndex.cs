namespace Etherna.BeeNet.Feeds
{
    public interface IFeedIndex
    {
        byte[] MarshalBinary { get; }
        IFeedIndex GetNext(ulong at);
    }
}