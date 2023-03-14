using System;
using System.Collections.ObjectModel;

namespace Etherna.BeeNet.Feeds.Models
{
    public class FeedChunk
    {
        public FeedChunk(
            FeedIndexBase index,
            byte[] payload,
            string referenceHash)
        {
            Index = index;
            Payload = Array.AsReadOnly(payload);
            ReferenceHash = referenceHash;
        }

        public FeedIndexBase Index { get; }
        public ReadOnlyCollection<byte> Payload { get; }
        public string ReferenceHash { get; }
    }
}
