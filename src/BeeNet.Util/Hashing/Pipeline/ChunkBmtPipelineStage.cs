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
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    /// <summary>
    /// Calculate hash of each chunk
    /// </summary>
    internal sealed class ChunkBmtPipelineStage(
        ushort compactLevel,
        IHasherPipelineStage nextStage,
        IPostageStampIssuer stampIssuer)
        : IHasherPipelineStage
    {
        // Fields.
        private readonly ConcurrentDictionary<long, object> lockObjectsDictionary = new(); //<chunkNumber, lockObj>
        
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

            var plainChunkHash = SwarmChunkBmtHasher.Hash(
                args.Data[..SwarmChunk.SpanSize].ToArray(),
                args.Data[SwarmChunk.SpanSize..].ToArray());
            if (compactLevel == 0)
            {
                /* If no chunk compaction is involved, simply calculate the chunk hash and proceed. */
                args.Hash = plainChunkHash;
            }
            else
            {
                /*
                 * If chunk compaction is involved, use optimistic chunk calculation.
                 * Calculate an encryption key, and try to find an optimal bucket collision.
                 * Before to proceed with the chunk and its key, wait optimistically until
                 * the previous chunk has been stored. Then verify if the same bucket has received
                 * new collisions, or not.
                 * If not, proceed.
                 * If yes, try again to search the best chunk key.
                 *
                 * The chunk key is calculate from the plain chunk hash, replacing the last 4 bytes
                 * with the attempt counter (int), and then hashing again.
                 * 
                 *     chunkKey = Keccack(plainChunkHash[..^2] + attempt)
                 *
                 * The encrypted chunk is calculated encrypting data with the chunk key.
                 *
                 * The optimistic algorithm will search the first best chunk available, trying a max of
                 * incremental attempts with max at the "compactionLevel" parameter.
                 *
                 * Best chunk is a chunk that fits in a bucket with the lowest possible number of collisions.
                 *
                 * If after an optimistic wait we found a collision has happened into the same bucket,
                 * simply search again the best chunk. At this point we can't do any assumption, and the new first
                 * best chunk could use and already calculated chunkKey (can't say what), or could be still to
                 * be calculated.
                 *
                 * Use a cache on generated chunkKey and relative bucketId for each attempt. This permits to not
                 * repeat the hash calculation in case that we need to repeat the search.
                 */
                
                // Search best chunk key.
                var encryptionCache = new Dictionary<ushort /*attempt*/, (XorEncryptKey ChunkKey, uint BucketId)>();
                var (chunkKey, expectedCollisions) = TrySearchFirstBestChunkKey(args, encryptionCache, plainChunkHash);

                // Coordinate tasks execution in order.
                var lockObj = new object();
                lock (lockObj)
                {
                    //add current lockObj in dictionary, in order can be found by next task
                    lockObjectsDictionary.TryAdd(args.NumberId, lockObj);

                    //if it's not the first task, try to lock on previous chunk's lockObj
                    if (args.NumberId > 0)
                    {
                        //get lockObj from previous chunk, and remove from dictionary to clean up.
                        //at this point, it must be already locked by prev task, and maybe be released.
                        object prevLockObj;
                        while (!lockObjectsDictionary.TryRemove(args.NumberId - 1, out prevLockObj!))
                            Task.Delay(1);
                        
                        //wait until it is released from prev task
                        lock (prevLockObj)
                        {
                            // Check the optimistic result, and if it has been invalidated, do it again.
                            
                            //TODO
                        }
                    }
                    else
                    {
                        // Because this is the first chunk, we don't need to wait any previous chunk to complete.
                        // Moreover, any result with optimistic calculation must be valid, because no previous chunks
                        // can have invalidated the result. So we can simply proceed.
                        
                        //TODO
                    }
                }
            }

            await nextStage.FeedAsync(args).ConfigureAwait(false);
        }
        
        public Task<SwarmHash> SumAsync() => nextStage.SumAsync();
        
        // Helpers.
        private (XorEncryptKey ChunkKey, uint ExpectedCollisions) TrySearchFirstBestChunkKey(
            HasherPipelineFeedArgs args,
            Dictionary<ushort /*attempt*/, (XorEncryptKey ChunkKey, uint BucketId)> encryptionCache,
            SwarmHash plainChunkHash)
        {
            // Init.
            XorEncryptKey? bestChunkKey = default;
            uint bestCollisions = 0;
            
            var encryptedData = new byte[args.Data.Length - SwarmChunk.SpanSize];
            var hasher = new Hasher();
            var plainChunkHashArray = plainChunkHash.ToByteArray();
            var spanArray = args.Data[..SwarmChunk.SpanSize].ToArray();
                
            // Search best chunk key.
            for (ushort i = 0; i < compactLevel; i++)
            {
                XorEncryptKey chunkKey;
                uint collisions;
                
                if (encryptionCache.TryGetValue(i, out var cachedValues))
                {
                    chunkKey = cachedValues.ChunkKey;
                    collisions = stampIssuer.Buckets.GetCollisions(cachedValues.BucketId);
                }
                else
                {
                    // Create key.
                    BinaryPrimitives.WriteUInt16BigEndian(plainChunkHashArray.AsSpan()[..^2], i);
                    chunkKey = new XorEncryptKey(hasher.ComputeHash(plainChunkHashArray));
                    
                    // Encrypt data.
                    args.Data[SwarmChunk.SpanSize..].CopyTo(encryptedData);
                    chunkKey.EncryptDecrypt(encryptedData);
                    
                    // Calculate hash, bucket id, and save in cache.
                    var encryptedHash = SwarmChunkBmtHasher.Hash(spanArray, encryptedData);
                    var bucketId = encryptedHash.ToBucketId();
                    encryptionCache[i] = (chunkKey, bucketId);

                    // Check key collisions.
                    collisions = stampIssuer.Buckets.GetCollisions(bucketId);
                }
                
                // First attempt is always the best one.
                if (bestChunkKey is null)
                {
                    bestChunkKey = chunkKey;
                    bestCollisions = collisions;
                }
                
                // Check if collisions are optimal.
                if (collisions == stampIssuer.Buckets.MinBucketCollisions)
                    return (chunkKey, collisions);
                
                // Else, if this reach better collisions, but not the best.
                if (collisions < bestCollisions)
                {
                    bestChunkKey = chunkKey;
                    bestCollisions = collisions;
                }
            }

            return (bestChunkKey!, bestCollisions);
        }
    }
}