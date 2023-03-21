using Etherna.BeeNet.Feeds.Models;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Feeds
{
    public interface IFeedService
    {
        Task<FeedChunk> CreateNextEpochFeedChunkAsync(
            string account,
            byte[] topic,
            byte[] contentPayload,
            EpochFeedIndex? knownNearEpochIndex);

        Task<FeedChunk> CreateNextEpochFeedChunkAsync(
            byte[] account,
            byte[] topic,
            byte[] contentPayload,
            EpochFeedIndex? knownNearEpochIndex);

        /// <summary>
        /// Try to find epoch feed at a given time
        /// </summary>
        /// <param name="account">The ethereum account address</param>
        /// <param name="topic">The feed topic</param>
        /// <param name="at">The time to search</param>
        /// <param name="knownNearEpochIndex">Another known existing epoch index, near to looked time. Helps to perform lookup quicker</param>
        /// <returns>The found epoch feed chunk, or null</returns>
        Task<FeedChunk?> TryFindEpochFeedAsync(byte[] account, byte[] topic, DateTimeOffset at, EpochFeedIndex? knownNearEpochIndex);

        Task<FeedChunk?> TryFindEpochFeedAsync(string account, byte[] topic, DateTimeOffset at, EpochFeedIndex? knownNearEpochIndex);

        Task<FeedChunk?> TryGetFeedChunkAsync(byte[] account, byte[] topic, FeedIndexBase index);

        Task<FeedChunk?> TryGetFeedChunkAsync(string account, byte[] topic, FeedIndexBase index);

        Task<FeedChunk?> TryGetFeedChunkAsync(string chunkReference, FeedIndexBase index);
    }
}