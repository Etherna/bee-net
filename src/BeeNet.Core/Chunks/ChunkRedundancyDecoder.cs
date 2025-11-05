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
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Chunks
{
    public sealed class ChunkRedundancyDecoder : IDisposable
    {
        // Consts.
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
        
        // Fields.
        private readonly Dictionary<SwarmHash, int> cache = new();
        private readonly int[] inflight;
        private readonly SwarmShardReference[] references;
        private readonly int dataShardsAmount;
        private readonly IChunkStore destinationChunkStore;
        private readonly int parityShardsAmount;
        private readonly RedundancyStrategy strategy;
        private readonly bool strategyFallback;
        private readonly IReadOnlyChunkStore sourceChunkStore;
        private readonly TaskCompletionSource<bool>[] waits;

        private readonly TaskCompletionSource<bool> initRecoveryTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> recoveryTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private int failedCnt;
        private int fetchedCnt;
        private int lastLen;
        private byte[][] shardsBuffer;
        private SemaphoreSlim shardsBufferSemaphore = new(1, 1);

        // Constructor.
        public ChunkRedundancyDecoder(
            SwarmCac parentChunk,
            bool encryptedDataReferences,
            RedundancyStrategy strategy,
            bool strategyFallback,
            IReadOnlyChunkStore sourceChunkStore,
            IChunkStore destinationChunkStore)
        {
            ArgumentNullException.ThrowIfNull(parentChunk, nameof(parentChunk));
            
            references = parentChunk.GetIntermediateReferences(encryptedDataReferences);
            dataShardsAmount = references.Count(r => !r.IsParity);
            parityShardsAmount = references.Length - dataShardsAmount;
            
            inflight = new int[references.Length];
            this.strategy = strategy;
            this.strategyFallback = strategyFallback;
            this.sourceChunkStore = sourceChunkStore;
            this.destinationChunkStore = destinationChunkStore;
            waits = new TaskCompletionSource<bool>[references.Length];
            for (int i = 0; i < references.Length; i++)
            {
                if (!references[i].IsParity)
                    cache[references[i].Reference.Hash] = i;
                waits[i] = new TaskCompletionSource<bool>();
            }
            
            shardsBuffer = new byte[references.Length][];
        }
        
        // Dispose.
        public void Dispose()
        {
            shardsBufferSemaphore.Dispose();
        }
        
        // Methods.
        public async Task<SwarmCac> GetChunkAsync(SwarmHash hash, CancellationToken cancellationToken = default)
        {
            var i = cache[hash];
            await TryFetch(i, true, cancellationToken).ConfigureAwait(false);
            return new SwarmCac(hash, await TryGetData(i).ConfigureAwait(false));
        }
        
        public async Task PrefetchAsync(CancellationToken cancellationToken)
        {
            var succeeded = false;
            try
            {
                // Run redundancy strategies.
                var strategySucceeded = false;
                for (int s = (int)strategy; s <= (int)RedundancyStrategy.Race; s++)
                {
                    strategySucceeded = await TryRunStrategyAsync((RedundancyStrategy)s, cancellationToken)
                        .ConfigureAwait(false);
                    if (strategySucceeded || !strategyFallback)
                        break;
                }

                if (!strategySucceeded)
                    throw new KeyNotFoundException("Can't fetch child chunks");

                // Recover missing data chunks.
                initRecoveryTcs.SetResult(true);
                await RecoverAsync().ConfigureAwait(false);

                // Set as succeeded
                succeeded = true;
            }
            finally
            {
                recoveryTcs.SetResult(succeeded);
            }
        }
        
        // Helpers.
        private bool Fly(int i) => Interlocked.CompareExchange(ref inflight[i], 1, 0) == 0;
        
        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        private async Task<bool> TryFetch(
            int i,
            bool waitForRecovery,
            CancellationToken cancellationToken)
        {
            // Recovery has started, wait for result instead of fetching from the network.
            if (initRecoveryTcs.Task.IsCompleted)
            {
                if (waitForRecovery)
                    return await WaitRecoveryAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }

            // first time
            if (Fly(i))
            {
                using var timeoutCts = new CancellationTokenSource(DefaultTimeout);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                // Run guard to cancel fetch if recovery is initiated.
                if (waitForRecovery)
                {
                    _ = initRecoveryTcs.Task.ContinueWith(
                        static (_, cts) =>
                        {
                            try { ((CancellationTokenSource)cts!).Cancel(); }
                            catch (ObjectDisposedException) { }
                        },
                        linkedCts,
                        CancellationToken.None,
                        TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                }
                
                // retrieval
                SwarmChunk ch;
                try
                {
                    ch = await sourceChunkStore.GetAsync(references[i].Reference.Hash, false, linkedCts.Token).ConfigureAwait(false);
                    await SetDataAsync(i, ch.GetFullPayload()).ConfigureAwait(false);
                    waits[i].SetResult(true);
                    Interlocked.Add(ref fetchedCnt, 1);
                    return true;
                }
                catch
                {
                    Interlocked.Add(ref failedCnt, 1);
                    waits[i].SetResult(false);
                    if (waitForRecovery)
                        return await WaitRecoveryAsync(cancellationToken).ConfigureAwait(false);
                    return false;
                }
            }
            
            await waits[i].Task.ConfigureAwait(false);

            if (TryGetData(i) != null)
                return true;

            if (waitForRecovery)
                return await WaitRecoveryAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }

        private async Task<byte[]?> TryGetData(int i)
        {
            await shardsBufferSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                return i == dataShardsAmount - 1 && lastLen > 0 ? shardsBuffer[i][..lastLen] : shardsBuffer[i];
            }
            finally
            {
                shardsBufferSemaphore.Release();
            }
        }
        
        private async Task RecoverAsync()
        {
            // recover wraps the stages of data shard recovery:
            // 1. gather missing data shards
            // 2. decode using Reed-Solomon decoder
            // 3. save reconstructed chunks
            
            // gather missing shards
            var m = new List<int>();
            for (int i = 0; i < dataShardsAmount; i++)
                if (TryGetData(i) == null)
                    m.Add(i);
            
            // if recovery is not needed as there are no missing data chunks
            if (m.Count == 0)
                return;

            // decode uses Reed-Solomon erasure coding decoder to recover data shards
            // it must be called after shqrdcnt shards are retrieved
            await shardsBufferSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var reedSolomonEncoder = ReedSolomon.NET.ReedSolomon.Create(dataShardsAmount, parityShardsAmount);

                var shardsPresent = new bool[shardsBuffer.Length];
                for (int i = 0; i < shardsBuffer.Length; i++)
                {
                    if (shardsBuffer[i] == null)
                        shardsBuffer[i] = new byte[SwarmCac.SpanDataSize];
                    else
                        shardsPresent[i] = true;
                }
                
                // decode data
                reedSolomonEncoder.DecodeMissing(shardsBuffer, shardsPresent, 0, SwarmCac.SpanDataSize);
            }
            finally
            {
                shardsBufferSemaphore.Release();
            }

            // save chunks
            await SaveAsync(m).ConfigureAwait(false);
        }

        private async Task SaveAsync(List<int> missing)
        {
            await shardsBufferSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var i in missing)
                {
                    await destinationChunkStore.AddAsync(new SwarmCac(references[i].Reference.Hash, shardsBuffer[i])).ConfigureAwait(false);
                }
            }
            finally
            {
                shardsBufferSemaphore.Release();
            }
        }

        private async Task SetDataAsync(int i, ReadOnlyMemory<byte> chdata)
        {
            await shardsBufferSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var data = chdata.ToArray();
            
                // pad the chunk with zeros if it is smaller than swarm.ChunkSize
                if (data.Length < SwarmCac.SpanDataSize)
                {
                    lastLen = data.Length;
                    data = new byte[SwarmCac.SpanDataSize];
                    chdata.CopyTo(data);
                }
                
                shardsBuffer[i] = data;
            }
            finally
            {
                shardsBufferSemaphore.Release();
            }
        }

        private async Task<bool> TryRunStrategyAsync(
            RedundancyStrategy strategy,
            CancellationToken cancellationToken)
        {
            /*
             * Across the different strategies, the common goal is to fetch at least as many chunks
             * as the number of data shards.
             * DATA strategy has a max error tolerance of zero.
             * RACE strategy has a max error tolerance of number of parity chunks.
             */
            int allowedErrs;
            List<int> m;

            switch (strategy)
            {
                case RedundancyStrategy.None:
                    throw new ArgumentException($"Strategy {strategy} not allowed here", nameof(strategy));
                
                case RedundancyStrategy.Data: //only retrieve data shards
                    m = UnattemptedDataShards();
                    allowedErrs = 0;
                    break;
                
                case RedundancyStrategy.Prox: //proximity driven selective fetching
                    return false; //TODO: strategy not implemented
                
                case RedundancyStrategy.Race:
                    allowedErrs = parityShardsAmount;
                    
                    //retrieve all chunks at once enabling race among chunks
                    m = UnattemptedDataShards();
                    m.AddRange(Enumerable.Range(dataShardsAmount, parityShardsAmount));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }

            if (m.Count == 0)
                return true;

            List<Task<bool>> fetchTasks = [..m.Select(i => TryFetch(i, false, cancellationToken))];
            while (fetchTasks.Count != 0)
            {
                var completedTask = await Task.WhenAny(fetchTasks).ConfigureAwait(false);
                fetchTasks.Remove(completedTask);

                if (fetchedCnt >= dataShardsAmount) return true;
                if (failedCnt > allowedErrs) return false;
            }
            
            throw new InvalidOperationException("Should never reach this point");
        }
        
        private List<int> UnattemptedDataShards()
        {
            var m = new List<int>();
            for (int i = 0; i < dataShardsAmount; i++)
            {
                if (!waits[i].Task.IsCompleted)
                    m.Add(i);
            }
            return m;
        }
        
        private async Task<bool> WaitRecoveryAsync(CancellationToken cancellationToken)
        {
            var cancelTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var regTask = cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s!).TrySetResult(true),
                cancelTcs);
            await using var reg = regTask.ConfigureAwait(false);

            var completedTask = await Task.WhenAny(recoveryTcs.Task, cancelTcs.Task).ConfigureAwait(false);
            
            // If cancellation token has been canceled, return false.
            if (completedTask == cancelTcs.Task)
                return false;
                
            // Else, return recovery result.
            return await recoveryTcs.Task.ConfigureAwait(false);
        }
    }
}