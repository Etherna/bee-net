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
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    /// <summary>
    /// A chunk store that reads from the first chunk store, and mirrors changes also to secondary chunk stores
    /// </summary>
    public class MirroringChunkStore : IChunkStore
    {
        // Fields.
        private readonly IChunkStore firstChunkStore;
        private readonly IChunkStore[] mirroredChunkStores;

        // Constructor.
        public MirroringChunkStore(
            IChunkStore firstChunkStore,
            params IChunkStore[] mirroredChunkStores)
        {
            ArgumentNullException.ThrowIfNull(firstChunkStore);
            ArgumentNullException.ThrowIfNull(mirroredChunkStores);

            this.firstChunkStore = firstChunkStore;
            this.mirroredChunkStores = mirroredChunkStores;
        }

        // Methods.
        public async Task<bool> AddAsync(
            SwarmChunk chunk,
            CancellationToken cancellationToken = default)
        {
            var result = await firstChunkStore.AddAsync(chunk, cancellationToken).ConfigureAwait(false);
            foreach (var chunkStore in mirroredChunkStores)
                await chunkStore.AddAsync(chunk, cancellationToken).ConfigureAwait(false);
            return result;
        }
        
        public Task<SwarmChunk> GetAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            firstChunkStore.GetAsync(hash, cancellationToken);

        public Task<IReadOnlyDictionary<SwarmHash, SwarmChunk?>> GetAsync(
            IEnumerable<SwarmHash> hashes,
            int? canReturnAfterFailed = null,
            int? canReturnAfterSucceeded = null,
            CancellationToken cancellationToken = default) =>
            firstChunkStore.GetAsync(
                hashes, canReturnAfterFailed, canReturnAfterSucceeded, cancellationToken);

        public Task<bool> HasChunkAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            firstChunkStore.HasChunkAsync(hash, cancellationToken);

        public Task<SwarmChunk?> TryGetAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            firstChunkStore.TryGetAsync(hash, cancellationToken);

        public async Task<bool> RemoveAsync(SwarmHash hash, CancellationToken cancellationToken = default)
        {
            var result = await firstChunkStore.RemoveAsync(hash, cancellationToken).ConfigureAwait(false);
            foreach (var chunkStore in mirroredChunkStores)
                await chunkStore.RemoveAsync(hash, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}