// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Feeds;
using Etherna.BeeNet.Models;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public interface IFeedService
    {
        Task<SwarmFeedChunk> CreateNextEpochFeedChunkAsync(
            string account,
            byte[] topic,
            byte[] contentPayload,
            EpochFeedIndex? knownNearEpochIndex);

        Task<SwarmFeedChunk> CreateNextEpochFeedChunkAsync(
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
        Task<SwarmFeedChunk?> TryFindEpochFeedAsync(byte[] account, byte[] topic, DateTimeOffset at, EpochFeedIndex? knownNearEpochIndex);

        Task<SwarmFeedChunk?> TryFindEpochFeedAsync(string account, byte[] topic, DateTimeOffset at, EpochFeedIndex? knownNearEpochIndex);

        Task<SwarmFeedChunk?> TryGetFeedChunkAsync(byte[] account, byte[] topic, FeedIndexBase index);

        Task<SwarmFeedChunk?> TryGetFeedChunkAsync(string account, byte[] topic, FeedIndexBase index);

        Task<SwarmFeedChunk?> TryGetFeedChunkAsync(SwarmHash hash, FeedIndexBase index);
    }
}