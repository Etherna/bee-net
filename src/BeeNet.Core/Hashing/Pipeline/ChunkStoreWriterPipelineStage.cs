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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    internal sealed class ChunkStoreWriterPipelineStage(
        IChunkStore chunkStore,
        IPostageStamper postageStamper,
        IHasherPipelineStage? nextStage,
        bool readOnly)
        : IHasherPipelineStage
    {
        // Dispose.
        public void Dispose()
        {
            nextStage?.Dispose();
        }
        
        // Properties.
        public long MissedOptimisticHashing => nextStage?.MissedOptimisticHashing ?? 0;
        public IPostageStamper PostageStamper => postageStamper;

        // Methods.
        public async Task FeedAsync(HasherPipelineFeedArgs args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));
            if (args.Hash is null) throw new InvalidOperationException();

            if (!readOnly)
            {
                // Stamp chunk and store stamp.
                var stamp = postageStamper.Stamp(args.Hash.Value);
            
                // Store chunk.
                var chunk = SwarmChunk.BuildFromSpanAndData(args.Hash.Value, args.Data.Span);
                chunk.PostageStamp = stamp;
                await chunkStore.AddAsync(chunk).ConfigureAwait(false);
            }

            if (nextStage is not null)
                await nextStage.FeedAsync(args).ConfigureAwait(false);
        }
        
        public Task<SwarmChunkReference> SumAsync() =>
            nextStage?.SumAsync() ?? throw new InvalidOperationException();
    }
}