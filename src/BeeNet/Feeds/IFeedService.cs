using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Feeds
{
    public interface IFeedService
    {
        byte[] GetIdentifier(byte[] topic, IFeedIndex index);

        string GetReferenceHash(string account, byte[] topic, IFeedIndex index);

        string GetReferenceHash(string account, byte[] identifier);

        /// <summary>
        /// Try to find epoch feed at a given time
        /// </summary>
        /// <param name="account">The ethereum account address</param>
        /// <param name="topic">The feed topic</param>
        /// <param name="at">The time to search</param>
        /// <returns>The found feed soc hash and its payload stream</returns>
        Task<(string, Stream)?> TryFindEpochFeedAtAsync(string account, byte[] topic, DateTime at);

        Task<string> UpdateEpochFeedAt(DateTime at, string payload);
    }
}