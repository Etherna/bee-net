using System;
using System.Threading.Tasks;
using Etherna.BeeNet.Feeds.Models;

namespace Etherna.BeeNet.Feeds
{
    public interface IFeedService
    {
        byte[] GetIdentifier(byte[] topic, FeedIndexBase index);

        string GetReferenceHash(string account, byte[] topic, FeedIndexBase index);

        string GetReferenceHash(byte[] account, byte[] topic, FeedIndexBase index);

        string GetReferenceHash(string account, byte[] identifier);

        string GetReferenceHash(byte[] account, byte[] identifier);

        /// <summary>
        /// Try to find epoch feed at a given time
        /// </summary>
        /// <param name="account">The ethereum account address</param>
        /// <param name="topic">The feed topic</param>
        /// <param name="at">The time to search</param>
        /// <returns>The found epoch feed chunk, or null</returns>
        Task<FeedChunk?> TryFindEpochFeedAsync(string account, byte[] topic, DateTimeOffset at);

        Task<string> UpdateEpochFeed(DateTime at, string payload);
    }
}