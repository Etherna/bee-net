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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    /// <summary>
    /// Calculate hash of each chunk
    /// </summary>
    internal sealed class ChunkBmtPipelineStage : IHasherPipelineStage
    {
        // Consts.
        public const int MinCompactionLevel = 0;
        
        // Fields.
        private readonly int compactionLevel;
        private readonly IHasherPipelineStage nextStage;
        private readonly IPostageStampIssuer stampIssuer;

        // Constructor.
        public ChunkBmtPipelineStage(
            int compactionLevel,
            IHasherPipelineStage nextStage,
            IPostageStampIssuer stampIssuer)
        {
#pragma warning disable CA1512
            //disable warning because "ThrowIfLessThan()" is only supported since .Net8
            if (compactionLevel < MinCompactionLevel)
                throw new ArgumentOutOfRangeException(nameof(compactionLevel));
#pragma warning restore CA1512

            this.compactionLevel = compactionLevel;
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

            var plainChunkHash = SwarmChunkBmtHasher.Hash(
                args.Data[..SwarmChunk.SpanSize].ToArray(),
                args.Data[SwarmChunk.SpanSize..].ToArray());
            if (compactionLevel == 0)
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
                 *     chunkKey = Keccack(plainChunkHash[..-4] + attempt)
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
                 */
                //TODO
            }

            await nextStage.FeedAsync(args).ConfigureAwait(false);
        }

        public Task<SwarmHash> SumAsync() => nextStage.SumAsync();
    }
}