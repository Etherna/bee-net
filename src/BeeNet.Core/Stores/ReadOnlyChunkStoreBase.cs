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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public abstract class ReadOnlyChunkStoreBase(
        IDictionary<SwarmHash, SwarmChunk>? chunksCache = null)
        : IReadOnlyChunkStore
    {
        // Properties.
        protected IDictionary<SwarmHash, SwarmChunk> ChunksCache { get; } =
            chunksCache ?? new Dictionary<SwarmHash, SwarmChunk>();

        // Methods.
        public async Task<SwarmChunk> GetAsync(
            SwarmHash hash,
            bool cacheChunk = false)
        {
            if (ChunksCache.TryGetValue(hash, out var chunk))
                return chunk;

            chunk = await LoadChunkAsync(hash).ConfigureAwait(false);

            if (cacheChunk)
                ChunksCache[hash] = chunk;

            return chunk;
        }

        public async Task<SwarmChunk?> TryGetAsync(
            SwarmHash hash,
            bool cacheChunk = false)
        {
            try
            {
                return await GetAsync(hash, cacheChunk).ConfigureAwait(false);
            }
            catch (BeeNetApiException)
            {
                return null;
            }
        }

        // Protected methods.
        protected abstract Task<SwarmChunk> LoadChunkAsync(SwarmHash hash);
    }
}