﻿//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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