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
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public abstract class ChunkStoreBase(
        ConcurrentDictionary<SwarmHash, SwarmChunk>? chunksCache = null)
        : ReadOnlyChunkStoreBase(chunksCache), IChunkStore
    {
        /// <summary>
        /// Add a chunk in the store
        /// </summary>
        /// <param name="chunk">The chuck to add</param>
        /// <param name="cacheChunk">Add chunk in cache</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if chunk has been added, false if already existing</returns>
        public async Task<bool> AddAsync(
            SwarmChunk chunk,
            bool cacheChunk = false,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            
            var result = await SaveChunkAsync(chunk, cancellationToken).ConfigureAwait(false);
            
            if (cacheChunk)
                ChunksCache[chunk.Hash] = chunk;
            
            return result;
        }

        public Task<bool> RemoveAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default)
        {
            var result = RemoveChunkAsync(hash, cancellationToken);

            ChunksCache.Remove(hash);

            return result;
        }

        // Protected methods.
        protected abstract Task<bool> RemoveChunkAsync(SwarmHash hash, CancellationToken cancellationToken);
        protected abstract Task<bool> SaveChunkAsync(SwarmChunk chunk, CancellationToken cancellationToken);
    }
}