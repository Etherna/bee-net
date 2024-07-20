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

using Etherna.BeeNet.Hashing.Bmt;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    /// <summary>
    /// Calculate hash of each chunk
    /// </summary>
    internal sealed class ChunkBmtPipelineStage : IHasherPipelineStage
    {
        // Fields.
        private readonly ushort compactLevel;
        private readonly IHasherPipelineStage nextStage;
        private readonly IPostageStampIssuer stampIssuer;

        // Constructor.
        /// <summary>
        /// Calculate hash of each chunk
        /// </summary>
        public ChunkBmtPipelineStage(
            ushort compactLevel,
            IHasherPipelineStage nextStage,
            IPostageStampIssuer stampIssuer)
        {
            this.compactLevel = compactLevel;
            this.nextStage = nextStage;
            this.stampIssuer = stampIssuer;
        }

        // Dispose.
        public void Dispose()
        {
            nextStage.Dispose();
        }

        // Methods.
        public async Task FeedAsync(HasherPipelineFeedArgs args)
        {
            if (args.Data.Length < SwarmChunk.SpanSize)
                throw new InvalidOperationException("Data can't be shorter than span size here");
            if (args.Data.Length > SwarmChunk.SpanAndDataSize)
                throw new InvalidOperationException("Data can't be longer than chunk + span size here");
            
            // Create an instance for this specific task. Hasher is not thread safe.
            var hasher = new Hasher();

            var plainChunkHash = SwarmChunkBmtHasher.Hash(
                args.Data[..SwarmChunk.SpanSize].ToArray(),
                args.Data[SwarmChunk.SpanSize..].ToArray(),
                hasher);
            if (compactLevel == 0)
            {
                /* If no chunk compaction is involved, simply calculate the chunk hash and proceed. */
                args.Hash = plainChunkHash;
            }
            else
            {
                // Search best chunk key.
                var bestChunkResult = await GetBestChunkAsync(args, plainChunkHash, hasher).ConfigureAwait(false);
                
                args.ChunkKey = bestChunkResult.ChunkKey;
                args.Data = bestChunkResult.EncryptedData;
                args.Hash = bestChunkResult.Hash;
            }

            await nextStage.FeedAsync(args).ConfigureAwait(false);
        }

        public Task<SwarmChunkReference> SumAsync() => nextStage.SumAsync();
        
        // Helpers.
        private static void EncryptDecryptChunkData(XorEncryptKey chunkKey, byte[] data)
        {
            // Don't encrypt span, otherwise knowing the chunk length, we could reconstruct the key.
            chunkKey.EncryptDecrypt(data.AsSpan()[SwarmChunk.SpanSize..]);
        }
        
        private async Task<CompactedChunkAttemptResult> GetBestChunkAsync(
            HasherPipelineFeedArgs args,
            SwarmHash plainChunkHash,
            Hasher hasher)
        {
            /*
             * If chunk compaction is involved, use optimistic calculation.
             * 
             * Calculate an encryption key, and try to find a bucket with optimal collisions.
             * Before to proceed with the chunk and its key, wait optimistically until
             * the previous chunk has been completed. Then verify if the same bucket has received
             * new collisions, or not. If not, proceed, otherwise try again to search the best chunk key.
             *
             * The chunk key is calculate from the plain chunk hash, replacing the last 4 bytes
             * with the attempt counter (int), and then hashing again.
             * 
             *     chunkKey = Keccack(plainChunkHash[^2..] + attempt)
             *
             * The encrypted chunk is calculated encrypting data with the chunk key.
             *
             * The optimistic algorithm will search the first best chunk available, trying a max of
             * incremental attempts with max at the "compactionLevel" parameter.
             *
             * Best chunk is a chunk that fits in a bucket with the lowest possible number of collisions.
             *
             * Use a cache on generated chunkKey and relative bucketId for each attempt. This permits to not
             * repeat the hash calculation in case that we need to repeat the search.
             */
                
            // Run optimistically before prev chunk completion.
            var encryptionCache = new Dictionary<ushort /*attempt*/, CompactedChunkAttemptResult>();
            var (bestKeyAttempt, expectedCollisions) = SearchFirstBestChunkKey(args, encryptionCache, plainChunkHash, hasher);

            // If there isn't any prev chunk to wait, proceed with result.
            if (args.PrevChunkSemaphore == null)
                return encryptionCache[bestKeyAttempt];

            try
            {
                // Otherwise wait until prev chunk has been processed.
                await args.PrevChunkSemaphore.WaitAsync().ConfigureAwait(false);
            
                // Check the optimistic result, and if it has been invalidated, do it again.
                var bestBucketId = encryptionCache[bestKeyAttempt].Hash.ToBucketId();
                var actualCollisions = stampIssuer.Buckets.GetCollisions(bestBucketId);
            
                if (actualCollisions == expectedCollisions)
                    return encryptionCache[bestKeyAttempt];
                    
                var (newBestKeyAttempt, _) = SearchFirstBestChunkKey(args, encryptionCache, plainChunkHash, hasher);
                return encryptionCache[newBestKeyAttempt];
            }
            finally
            {
                //release prev chunk semaphore to reuse it
                args.PrevChunkSemaphore.Release();
            }
        }
        
        private (ushort BestKeyAttempt, uint ExpectedCollisions) SearchFirstBestChunkKey(
            HasherPipelineFeedArgs args,
            Dictionary<ushort /*attempt*/, CompactedChunkAttemptResult> optimisticCache,
            SwarmHash plainChunkHash,
            Hasher hasher)
        {
            // Init.
            ushort bestAttempt = 0;
            uint bestCollisions = 0;
            
            var encryptedData = new byte[args.Data.Length];
            var plainChunkHashArray = plainChunkHash.ToByteArray();
                
            // Search best chunk key.
            for (ushort i = 0; i < compactLevel; i++)
            {
                uint collisions;
                
                if (optimisticCache.TryGetValue(i, out var cachedValues))
                {
                    collisions = stampIssuer.Buckets.GetCollisions(cachedValues.Hash.ToBucketId());
                }
                else
                {
                    // Create key.
                    BinaryPrimitives.WriteUInt16BigEndian(plainChunkHashArray.AsSpan()[^2..], i);
                    var chunkKey = new XorEncryptKey(hasher.ComputeHash(plainChunkHashArray));
                    
                    // Encrypt data.
                    args.Data.CopyTo(encryptedData);
                    EncryptDecryptChunkData(chunkKey, encryptedData);
                    
                    // Calculate hash, bucket id, and save in cache.
                    var encryptedHash = SwarmChunkBmtHasher.Hash(
                        encryptedData[..SwarmChunk.SpanSize],
                        encryptedData[SwarmChunk.SpanSize..],
                        hasher);
                    optimisticCache[i] = new(chunkKey, encryptedData, encryptedHash);

                    // Check key collisions.
                    collisions = stampIssuer.Buckets.GetCollisions(encryptedHash.ToBucketId());
                }
                
                // First attempt is always the best one.
                if (i == 0)
                    bestCollisions = collisions;
                
                // Check if collisions are optimal.
                if (collisions == stampIssuer.Buckets.MinBucketCollisions)
                    return (i, collisions);
                
                // Else, if this reach better collisions, but not the best.
                if (collisions < bestCollisions)
                {
                    bestAttempt = i;
                    bestCollisions = collisions;
                }
            }

            return (bestAttempt, bestCollisions);
        }
    }
}