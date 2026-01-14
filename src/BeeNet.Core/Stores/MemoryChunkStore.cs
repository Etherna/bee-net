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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public class MemoryChunkStore : ReadOnlyChunkStoreBase, IChunkStore
    {
        // Fields.
        private readonly ConcurrentDictionary<SwarmHash, SwarmChunk> chunks = new();
        
        // Properties.
        public IReadOnlyDictionary<SwarmHash, SwarmChunk> AllChunks => chunks;
        
        // Methods.
        public Task<bool> AddAsync(SwarmChunk chunk, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunk);
            
            return Task.FromResult(chunks.TryAdd(chunk.Hash, chunk));
        }
        
        public override Task<SwarmChunk> GetAsync(SwarmHash hash, CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.FromResult(chunks[hash]);
            }
            catch (KeyNotFoundException ex)
            {
                return Task.FromException<SwarmChunk>(ex);
            }
        }

        public override Task<bool> HasChunkAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            Task.FromResult(chunks.ContainsKey(hash));

        public Task<bool> RemoveAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            Task.FromResult(chunks.Remove(hash, out _));
    }
}