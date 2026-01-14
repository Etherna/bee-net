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

using Etherna.BeeNet.Chunks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using System.Linq;

namespace Etherna.BeeNet.Stores
{
    public class ReplicaResolverChunkStore(
        IReadOnlyChunkStore sourceChunkStore,
        RedundancyLevel level,
        Hasher hasher,
        TimeSpan? customLevelDelay = null)
        : ReadOnlyChunkStoreBase
    {
        // Fields.
        /// <summary>
        /// Duration between successive additional requests
        /// </summary>
        private readonly TimeSpan levelDelay = customLevelDelay ?? TimeSpan.FromMilliseconds(300);

        // Methods.
        public override async Task<SwarmChunk> GetAsync(SwarmHash hash, CancellationToken cancellationToken = default)
        {
            // Without redundancy, simply bypass.
            if (level == RedundancyLevel.None)
                return await sourceChunkStore.GetAsync(hash, cancellationToken).ConfigureAwait(false);
            
            // Otherwise, try to resolve with replicas.
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            try
            {
                var replicaHeaders = ChunkReplicator.GenerateReplicaHeaders(hash, level, hasher);
                
                // Concurrently try to retrieve original chunk with its replicas, using cumulative delays by level.
                // Requests (for level == 4):
                //   delay * 0: original hash
                //   delay * 1: +2 replicas
                //   delay * 2: +2 replicas (tot 4)
                //   delay * 3: +4 replicas (tot 8)
                //   delay * 4: +8 replicas (tot 16)
                List<Task<SwarmChunk>> getChunkTasks = [sourceChunkStore.GetAsync(hash, cts.Token)];
                foreach (var (replicaHeader, i) in replicaHeaders.Select((r, i) => (r, i)))
                {
                    var delayMultiplier = 0;
                    var j = i; //i is immutable
                    do
                    {
                        delayMultiplier++;
                        j >>= 1;
                    } while (j > 0);
                    getChunkTasks.Add(FetchReplicaAsync(replicaHeader.Hash, levelDelay * delayMultiplier, cts.Token));
                }
                
                // Returns the first replying with success.
                while (getChunkTasks.Count != 0)
                {
                    // Fetch parallelo con WhenAny per "first wins"
                    var completedTask = await Task.WhenAny(getChunkTasks).ConfigureAwait(false);
                    getChunkTasks.Remove(completedTask);

                    try
                    {
                        // If success, return and cancel other requests with finally block.
                        return await completedTask.ConfigureAwait(false);
                    }
                    catch (BeeNetApiException) { } // Ignore errors and try the next.
                    catch (InvalidOperationException) { }
                    catch (KeyNotFoundException) { }
                    catch (OperationCanceledException) { }
                }

                // Can't find chunk.
                throw new KeyNotFoundException("All search levels exhausted. Can't find chunk or replicas");
            }
            finally
            {
                await cts.CancelAsync().ConfigureAwait(false);
            }
        }

        // Helpers.
        private async Task<SwarmChunk> FetchReplicaAsync(
            SwarmHash replicaHash,
            TimeSpan delay,
            CancellationToken cancellationToken)
        {
            // Wait the delay for this level.
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            
            // Get replica chunk.
            var chunk = await sourceChunkStore.GetAsync(replicaHash, cancellationToken).ConfigureAwait(false);
            if (chunk is not SwarmSoc soc)
                throw new SwarmChunkTypeException(chunk, "Chunk is not a SOC");
            return soc.InnerChunk;
        }
    }
}
