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
    /// <summary>
    /// Tries to recover data children of an intermediate chunk using redundancy
    /// </summary>
    public sealed class ChunkRedundancyDecoder : IDisposable
    {
        // Consts.
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
        
        // Fields.
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly Dictionary<SwarmHash, int> hashIndexMap = new();

        private readonly ReaderWriterLockSlim recoveryLock = new(LockRecursionPolicy.NoRecursion);
        
        private readonly SwarmShardReference[] _shardReferences;
        private readonly byte[][] shardsBuffer;
        private readonly ReaderWriterLockSlim shardsBufferLock = new(LockRecursionPolicy.NoRecursion);
        private int lastChunkLength;
        
        private readonly int dataShardsAmount;
        
        // Constructor.
        public ChunkRedundancyDecoder(
            SwarmCac parentChunk,
            bool encryptedDataReferences,
            IReadOnlyChunkStore chunkStore)
        {
            ArgumentNullException.ThrowIfNull(parentChunk, nameof(parentChunk));
            
            this.chunkStore = chunkStore;
            
            _shardReferences = parentChunk.GetIntermediateReferences(encryptedDataReferences);
            shardsBuffer = new byte[_shardReferences.Length][];
            dataShardsAmount = _shardReferences.Count(r => !r.IsParity);
            
            for (int i = 0; i < _shardReferences.Length; i++)
                hashIndexMap[_shardReferences[i].Reference.Hash] = i;
        }
        
        // Dispose.
        public void Dispose()
        {
            recoveryLock.Dispose();
            shardsBufferLock.Dispose();
        }
        
        // Properties.
        public bool RecoverySucceeded { get; private set; }
        public ReadOnlySpan<SwarmShardReference> ShardReferences => _shardReferences;
        
        // Methods.
        public async Task AddChunksToStoreAsync(
            IChunkStore destinationStore,
            IEnumerable<SwarmHash> hashes,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(destinationStore, nameof(destinationStore));
            ArgumentNullException.ThrowIfNull(hashes, nameof(hashes));
            
            foreach (var hash in hashes)
            {
                var chunk = GetChunk(hash);
                await destinationStore.AddAsync(chunk, false, cancellationToken).ConfigureAwait(false);
            }
        }

        public SwarmCac GetChunk(SwarmHash hash)
        {
            shardsBufferLock.EnterReadLock();
            try
            {
                var i = hashIndexMap[hash];
                if (shardsBuffer[i] == null!)
                    throw new InvalidOperationException($"Chunk with hash {hash} has not been recovered");
                    
                var chunkSpanData = i == dataShardsAmount - 1 && lastChunkLength > 0 ?
                    shardsBuffer[i][..lastChunkLength] :
                    shardsBuffer[i];
        
                return new SwarmCac(hash, chunkSpanData);
            }
            finally
            {
                shardsBufferLock.ExitReadLock();
            }
        }
        
        public SwarmShardReference[] GetMissingShards()
        {
            shardsBufferLock.EnterReadLock();
            try
            {
                return shardsBuffer
                    .Zip(_shardReferences)
                    .Where(zip => zip.First == null!)
                    .Select(zip => zip.Second).ToArray();
            }
            finally
            {
                shardsBufferLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Try to retrieve enough missing chunks to perform redundancy recovery.
        /// </summary>
        /// <param name="firstStrategy">First strategy to try. None is not allowed</param>
        /// <param name="strategyFallback">When true and a strategy fails, fallback to try the next</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Succeeded recovery</returns>
        public async Task<bool> TryFetchChunksAsync(
            RedundancyStrategy firstStrategy,
            bool strategyFallback,
            CancellationToken cancellationToken)
        {
            // Verify if recovery has already been completed with success.
            recoveryLock.EnterReadLock();
            try
            {
                if (RecoverySucceeded)
                    return true;
            }
            finally
            {
                recoveryLock.ExitReadLock();
            }
            
            // Init chunk fetch tasks list with already found chunks.
            bool?[] fetchResults;
            shardsBufferLock.EnterReadLock();
            try
            {
                fetchResults = shardsBuffer.Select(b => (bool?)(b != null! ? true : null)).ToArray();
            }
            finally
            {
                shardsBufferLock.ExitReadLock();
            }
            
            // Run redundancy strategies.
            var succeeded = false;
            for (int s = (int)firstStrategy; !succeeded && s <= (int)RedundancyStrategy.Race; s++)
            {
                var strategy = (RedundancyStrategy)s;
                
                succeeded = await TryRunStrategyAsync(strategy, fetchResults, cancellationToken).ConfigureAwait(false);
                if (!strategyFallback)
                    break;
            }
        
            return succeeded;
        }
        
        /// <summary>
        /// Try to perform redundancy recovery.
        /// </summary>
        /// <param name="forceRecoverParities">When false skip recovery if all data chunks are present</param>
        /// <returns>Succeeded recovery</returns>
        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public bool TryRecoverChunks(bool forceRecoverParities)
        {
            // Verify if recovery has already been completed with success.
            recoveryLock.EnterReadLock();
            try
            {
                if (RecoverySucceeded)
                    return true;
            }
            finally
            {
                recoveryLock.ExitReadLock();
            }
            
            // If all data shards are not null, recovery is not needed.
            if (!forceRecoverParities)
            {
                shardsBufferLock.EnterReadLock();
                try
                {
                    if (shardsBuffer.Take(dataShardsAmount).All(s => s != null!))
                        return true;
                }
                finally
                {
                    shardsBufferLock.ExitReadLock();
                }
            }
            
            // Run actual recovery.
            recoveryLock.EnterWriteLock();
            try
            {
                // Use Reed-Solomon erasure coding decoder to recover data shards.
                var reedSolomonEncoder = ReedSolomon.NET.ReedSolomon.Create(
                    dataShardsAmount,
                    shardsBuffer.Length - dataShardsAmount);
                
                var shardsPresent = new bool[shardsBuffer.Length];
                
                shardsBufferLock.EnterWriteLock();
                try
                {
                    for (int i = 0; i < shardsBuffer.Length; i++)
                    {
                        if (shardsBuffer[i] == null!)
                            shardsBuffer[i] = new byte[SwarmCac.SpanDataSize];
                        else
                            shardsPresent[i] = true;
                    }
        
                    //decode missing shards
                    reedSolomonEncoder.DecodeMissing(shardsBuffer, shardsPresent, 0, SwarmCac.SpanDataSize);
                }
                catch
                {
                    // Restore missing chunks in buffer in case of error.
                    for (int i = 0; i < shardsBuffer.Length; i++)
                        if (!shardsPresent[i])
                            shardsBuffer[i] = null!;
                    
                    return false;
                }
                finally
                {
                    shardsBufferLock.ExitWriteLock();
                }
                
                // Return as succeeded.
                return true;
            }
            finally
            {
                recoveryLock.ExitWriteLock();
            }
        }
        
        // Helpers.
        
        /// <summary>
        /// Common goal is to fetch at least as many chunks as the number of data shards.
        /// DATA strategy has a max error tolerance of zero.
        /// RACE strategy has a max error tolerance of number of parity chunks.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="fetchResults"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> TryRunStrategyAsync(
            RedundancyStrategy strategy,
            bool?[] fetchResults,
            CancellationToken cancellationToken)
        {
            // Define allowed errors and hashes to query, based on strategy.
            var allowedErrors = 0;
            SwarmHash[] hashesToQuery;
        
            switch (strategy)
            {
                case RedundancyStrategy.None:
                    throw new ArgumentException($"Strategy None is not allowed here", nameof(strategy));
                
                case RedundancyStrategy.Data: //only retrieve data shards
                    hashesToQuery = fetchResults
                        .Zip(_shardReferences.Where(sr => !sr.IsParity))
                        .Where(zip => zip.First == null)
                        .Select(zip => zip.Second.Reference.Hash)
                        .ToArray();
                    break;
                
                case RedundancyStrategy.Prox: //proximity driven selective fetching
                    return false; //TODO: strategy not implemented
                
                case RedundancyStrategy.Race:
                    allowedErrors = _shardReferences.Length - dataShardsAmount;
                    hashesToQuery = fetchResults
                        .Zip(_shardReferences)
                        .Where(zip => zip.First == null)
                        .Select(zip => zip.Second.Reference.Hash)
                        .ToArray();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
            
            // Define and verify requirements.
            var succeededFetches = 0;
            var failedFetches = 0;
            foreach (var result in fetchResults)
            {
                switch (result)
                {
                    case true: succeededFetches++; break;
                    case false: failedFetches++; break;
                }
            }
            if (succeededFetches >= dataShardsAmount)
                return true;
            if (failedFetches > allowedErrors)
                return false;
        
            // Run chunks query.
            using var timeoutCts = new CancellationTokenSource(DefaultTimeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            var results = await chunkStore.GetAsync(
                hashesToQuery,
                canReturnAfterFailed: allowedErrors + 1,
                canReturnAfterSucceeded: dataShardsAmount - succeededFetches,
                cancellationToken: linkedCts.Token).ConfigureAwait(false);
            
            // Verify results.
            shardsBufferLock.EnterWriteLock();
            try
            {
                foreach (var hash in hashesToQuery)
                {
                    if (!results.TryGetValue(hash, out var chunk))
                        continue;

                    var shardIndex = hashIndexMap[hash];
                    if (chunk != null)
                    {
                        // Store found chunk. Pad data with zeros if it is smaller than SpanDataSize.
                        var spanData = new byte[SwarmCac.SpanDataSize];
                        chunk.GetFullPayload().CopyTo(spanData);
                        shardsBuffer[shardIndex] = spanData;
                        
                        var dataLength = chunk.GetFullPayload().Length;
                        if (dataLength < SwarmCac.SpanDataSize)
                            lastChunkLength = dataLength;
                        
                        // Update results.
                        fetchResults[shardIndex] = true;
                        succeededFetches++;
                    }
                    else
                    {
                        // Update results.
                        fetchResults[shardIndex] = false;
                        failedFetches++;
                    }
                }
            }
            finally
            {
                shardsBufferLock.ExitWriteLock();
            }
            
            return succeededFetches >= dataShardsAmount;
        }
    }
}