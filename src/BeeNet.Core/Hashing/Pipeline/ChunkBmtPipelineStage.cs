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

using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    /// <summary>
    /// Calculate hash of each chunk
    /// </summary>
    internal sealed class ChunkBmtPipelineStage(
        ushort compactLevel,
        IHasherPipelineStage nextStage)
        : IHasherPipelineStage
    {
        // Fields.
        private long _missedOptimisticHashing;

        // Dispose.
        public void Dispose()
        {
            nextStage.Dispose();
        }
        
        // Properties.
        public long MissedOptimisticHashing =>
            _missedOptimisticHashing + nextStage.MissedOptimisticHashing;
        public IPostageStamper PostageStamper => nextStage.PostageStamper;

        // Methods.
        public async Task FeedAsync(HasherPipelineFeedArgs args)
        {
            if (args.Data.Length < SwarmChunk.SpanSize)
                throw new InvalidOperationException("Data can't be shorter than span size here");
            if (args.Data.Length > SwarmChunk.SpanAndDataSize)
                throw new InvalidOperationException("Data can't be longer than chunk + span size here");
            
            // Create an instance for this specific task. Hasher is not thread safe.
            var chunkBmt = new SwarmChunkBmt(args.Hasher);
            var plainChunkHash = chunkBmt.Hash(
                args.Data[..SwarmChunk.SpanSize].ToArray(),
                args.Data[SwarmChunk.SpanSize..].ToArray());
            if (compactLevel == 0)
            {
                /* If no chunk compaction is involved, simply calculate the chunk hash and proceed. */
                args.Hash = plainChunkHash;
            }
            else
            {
                // Search best chunk key.
                var bestChunkResult = await GetBestChunkAsync(args, plainChunkHash).ConfigureAwait(false);
                
                args.ChunkKey = bestChunkResult.ChunkKey;
                args.Data = bestChunkResult.EncryptedData;
                args.Hash = bestChunkResult.Hash;
            }

            await nextStage.FeedAsync(args).ConfigureAwait(false);
        }

        public Task<SwarmChunkReference> SumAsync(IHasher hasher) => nextStage.SumAsync(hasher);
        
        // Helpers.
        private static void EncryptDecryptChunkData(XorEncryptKey chunkKey, byte[] data)
        {
            // Don't encrypt span, otherwise knowing the chunk length, we could reconstruct the key.
            chunkKey.EncryptDecrypt(data.AsSpan()[SwarmChunk.SpanSize..]);
        }
        
        private async Task<CompactedChunkAttemptResult> GetBestChunkAsync(
            HasherPipelineFeedArgs args,
            SwarmHash plainChunkHash)
        {
            /*
             * If chunk compaction is involved, use optimistic calculation.
             * 
             * Calculate an encryption key, and try to find a bucket with optimal collisions.
             * Before to proceed with the chunk and its key, wait optimistically until
             * the previous chunk has been completed. Then verify if the same bucket has received
             * new collisions, or not. If not, proceed, otherwise try again to search the best chunk key.
             *
             * The chunk key is calculated from the plain chunk hash, replacing the last 2 bytes
             * with the attempt counter (ushort), and then hashing again.
             * 
             *     chunkKey = Keccack(plainChunkHash[^2..] + attempt)
             *
             * Optimized chunk is calculated encrypting data with the chunk key.
             *
             * The optimistic algorithm will search the first best chunk available, trying a max of
             * incremental attempts with max at the "compactionLevel" parameter.
             *
             * Best chunk is a chunk that fits in a bucket with the lowest possible number of collisions.
             *
             * Use a cache on generated chunkKey and relative bucketId for each attempt. This permits to not
             * repeat the hash calculation in case that we need to repeat the search.
             *
             * In case that a chunk hash has already been stored into stamp store, accept it without optimistic check.
             * This is the best case of all, because it will not increment any bucket. Anyway, still wait
             * prev chunk completion because we want to avoid this otherwise possible case:
             * - sequential input chunks are [0, 1, 2, ...]
             * - chunk 1 is the only one with its hash previously stamped
             * - hashing of 0 keeps long time to complete,
             * - hashing of 1 immediately proceed because was already stamped, and
             * - from 2 onwards can proceed with optimization without need of wait for 0 to be inserted
             */
                
            // Run optimistically before prev chunk completion.
            var encryptionCache = new Dictionary<ushort /*attempt*/, CompactedChunkAttemptResult>();
            var (bestKeyAttempt, expectedCollisions, wasAlreadyStamped) =
                SearchFirstBestChunkKey(args, encryptionCache, plainChunkHash);

            // If there isn't any prev chunk to wait, proceed with result.
            if (args.PrevChunkSemaphore == null)
                return encryptionCache[bestKeyAttempt];

            try
            {
                // Otherwise wait until prev chunk has been processed.
                await args.PrevChunkSemaphore.WaitAsync().ConfigureAwait(false);
                
                // ** Here chunks can enter only once per time, and in order. **
                
                // If chunk was already stamped, keep it.
                if (wasAlreadyStamped)
                    return encryptionCache[bestKeyAttempt];
            
                // Check the optimistic result, and keep if valid.
                var bestBucketId = encryptionCache[bestKeyAttempt].Hash.ToBucketId();
                var actualCollisions = PostageStamper.StampIssuer.Buckets.GetCollisions(bestBucketId);
            
                if (actualCollisions == expectedCollisions)
                    return encryptionCache[bestKeyAttempt];

                // If it has been invalidated, do it again.
                _missedOptimisticHashing++;
                var (newBestKeyAttempt, _, _) = SearchFirstBestChunkKey(args, encryptionCache, plainChunkHash);
                return encryptionCache[newBestKeyAttempt];
            }
            finally
            {
                //release prev chunk semaphore to reuse it
                args.PrevChunkSemaphore.Release();
            }
        }
        
        private (ushort bestKeyAttempt, uint expectedCollisions, bool wasAlreadyStamped) SearchFirstBestChunkKey(
            HasherPipelineFeedArgs args,
            Dictionary<ushort /*attempt*/, CompactedChunkAttemptResult> optimisticCache,
            SwarmHash plainChunkHash)
        {
            // Init.
            ushort bestAttempt = 0;
            uint bestCollisions = 0;
            
            var plainChunkHashArray = plainChunkHash.ToByteArray();
                
            // Search best chunk key.
            for (ushort i = 0; i < compactLevel; i++)
            {
                uint collisions;
                
                if (optimisticCache.TryGetValue(i, out var cachedValues))
                {
                    collisions = PostageStamper.StampIssuer.Buckets.GetCollisions(cachedValues.Hash.ToBucketId());
                }
                else
                {
                    // Create key.
                    BinaryPrimitives.WriteUInt16BigEndian(plainChunkHashArray.AsSpan()[^2..], i);
                    var chunkKey = new XorEncryptKey(args.Hasher.ComputeHash(plainChunkHashArray));
                    
                    // Encrypt data.
                    var encryptedData = args.Data.ToArray();
                    EncryptDecryptChunkData(chunkKey, encryptedData);
                    
                    // Calculate hash, bucket id, and save in cache.
                    var chunkBmt = new SwarmChunkBmt(args.Hasher);
                    var encryptedHash = chunkBmt.Hash(
                        encryptedData[..SwarmChunk.SpanSize],
                        encryptedData[SwarmChunk.SpanSize..]);
                    optimisticCache[i] = new(chunkKey, encryptedData, encryptedHash);

                    // Check key collisions.
                    collisions = PostageStamper.StampIssuer.Buckets.GetCollisions(encryptedHash.ToBucketId());
                }
                
                // First attempt is always the best one.
                if (i == 0)
                    bestCollisions = collisions;
                
                // Check if hash was already stamped.
                if (PostageStamper.StampStore.TryGet(
                        StampStoreItem.BuildId(PostageStamper.StampIssuer.PostageBatch.Id, optimisticCache[i].Hash),
                        out _))
                    return (i, collisions, true);
                
                // Check if collisions are optimal.
                if (collisions == PostageStamper.StampIssuer.Buckets.MinBucketCollisions)
                    return (i, collisions, false);
                
                // Else, if this reach better collisions, but not the best.
                if (collisions < bestCollisions)
                {
                    bestAttempt = i;
                    bestCollisions = collisions;
                }
            }

            return (bestAttempt, bestCollisions, false);
        }
    }
}