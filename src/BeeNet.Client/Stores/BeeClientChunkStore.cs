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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public class BeeClientChunkStore(
        IBeeClient beeClient,
        IDictionary<SwarmHash, SwarmChunk>? chunksCache = null)
        : ReadOnlyChunkStoreBase(chunksCache)
    {
        // Fields.
        private readonly ConcurrentQueue<SwarmChunkBmt> swarmChunkBmtPool = new();
        
        // Methods.
        public override Task<bool> HasChunkAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            beeClient.IsChunkExistingAsync(hash, cancellationToken: cancellationToken);

        protected override async Task<SwarmChunk> LoadChunkAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default)
        {
            // Get chunk trying to reuse bmts with concurrency.
            if (!swarmChunkBmtPool.TryDequeue(out var swarmChunkBmt))
                swarmChunkBmt = new SwarmChunkBmt();
            
            var result = await beeClient.GetChunkAsync(
                hash,
                swarmChunkBmt,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            swarmChunkBmt.Clear();
            swarmChunkBmtPool.Enqueue(swarmChunkBmt);

            return result;
        }
    }
}