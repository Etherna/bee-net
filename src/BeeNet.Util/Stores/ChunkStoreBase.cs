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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public abstract class ChunkStoreBase(
        IDictionary<SwarmHash, SwarmChunk>? chunksCache = null)
        : ReadOnlyChunkStoreBase(chunksCache)
    {
        /// <summary>
        /// Add a chunk in the store
        /// </summary>
        /// <param name="chunk">The chuck to add</param>
        /// <param name="bypassCacheWriting">Bypass cache update</param>
        /// <returns>True if chunk has been added, false if already existing</returns>
        public async Task<bool> AddAsync(
            SwarmChunk chunk,
            bool bypassCacheWriting)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            
            var result = await SaveChunkAsync(chunk).ConfigureAwait(false);
            
            if (!bypassCacheWriting)
                ChunksCache[chunk.Hash] = chunk;
            
            return result;
        }
        
        // Protected methods.
        protected abstract Task<bool> SaveChunkAsync(SwarmChunk chunk);
    }
}