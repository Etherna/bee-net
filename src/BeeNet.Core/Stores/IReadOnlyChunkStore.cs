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

using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public interface IReadOnlyChunkStore
    {
        Task<SwarmChunk> GetAsync(
            SwarmHash hash,
            bool cacheChunk = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Try get multiple chunks
        /// </summary>
        /// <param name="hashes">Chunks' hashes to find</param>
        /// <param name="cacheChunk"></param>
        /// <param name="canReturnAfterFailed">If set, the function can return after have failed this number of chunks</param>
        /// <param name="canReturnAfterSucceeded">If set, the function can return after have found this number of chunks</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A dictionary listing found chunks by their hashes</returns>
        Task<IReadOnlyDictionary<SwarmHash, SwarmChunk?>> GetAsync(
            IEnumerable<SwarmHash> hashes,
            bool cacheChunk = false,
            int? canReturnAfterFailed = null,
            int? canReturnAfterSucceeded = null,
            CancellationToken cancellationToken = default);
        
        Task<bool> HasChunkAsync(SwarmHash hash, CancellationToken cancellationToken = default);

        Task<SwarmChunk?> TryGetAsync(
            SwarmHash hash,
            bool cacheChunk = false,
            CancellationToken cancellationToken = default);
    }
}