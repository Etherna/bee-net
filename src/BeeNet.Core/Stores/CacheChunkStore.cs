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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public class CacheChunkStore(
        IChunkStore sourceChunkStore,
        IDictionary<SwarmHash, SwarmChunk>? chunksCache = null)
        : ReadOnlyChunkStoreBase, IChunkStore
    {
        // Properties.
        private IDictionary<SwarmHash, SwarmChunk> ChunksCache { get; } =
            chunksCache ?? new Dictionary<SwarmHash, SwarmChunk>();

        // Methods.
        public Task<bool> AddAsync(SwarmChunk chunk, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunk);
            
            ChunksCache[chunk.Hash] = chunk;
            return sourceChunkStore.AddAsync(chunk, cancellationToken);
        }

        public override async Task<SwarmChunk> GetAsync(SwarmHash hash, CancellationToken cancellationToken = default)
        {
            if (ChunksCache.TryGetValue(hash, out var chunk))
                return chunk;

            chunk = await sourceChunkStore.GetAsync(hash, cancellationToken).ConfigureAwait(false);
            ChunksCache[hash] = chunk;

            return chunk;
        }

        public override async Task<IReadOnlyDictionary<SwarmHash, SwarmChunk?>> GetAsync(
            IEnumerable<SwarmHash> hashes,
            int? canReturnAfterFailed = null,
            int? canReturnAfterSucceeded = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(hashes);
            
            var results = new Dictionary<SwarmHash, SwarmChunk?>();
            var missedHashes = new List<SwarmHash>();
            
            // Try read chunks from cache.
            foreach (var hash in hashes)
                if (ChunksCache.TryGetValue(hash, out var chunk))
                    results.Add(hash, chunk);
                else
                    missedHashes.Add(hash);

            // Get from store only missing chunks.
            if (missedHashes.Count != 0 &&
                (!canReturnAfterSucceeded.HasValue || canReturnAfterSucceeded > results.Count))
            {
                var storeResults = await sourceChunkStore.GetAsync(
                    missedHashes,
                    canReturnAfterFailed,
                    canReturnAfterSucceeded.HasValue ? canReturnAfterSucceeded - results.Count : null,
                    cancellationToken).ConfigureAwait(false);
                foreach (var result in storeResults)
                    results.Add(result.Key, result.Value);
            }

            // Report chunks to cache, if required.
            foreach (var result in results.Where(r => r.Value != null))
                ChunksCache.TryAdd(result.Key, result.Value!);
            
            return results;
        }

        public Task<bool> RemoveAsync(SwarmHash hash, CancellationToken cancellationToken = default)
        {
            ChunksCache.Remove(hash);
            return sourceChunkStore.RemoveAsync(hash, cancellationToken);
        }
    }
}