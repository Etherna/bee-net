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
using Etherna.BeeNet.Hashing.Store;
using Etherna.BeeNet.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    public static class HasherPipelineBuilder
    {
        // Static builders.
        public static IHasherPipeline BuildNewHasherPipeline(
            IPostageStamper postageStamper,
            RedundancyLevel redundancyLevel,
            bool isEncrypted,
            string? chunkStoreDirectory,
            int compactionLevel) =>
            BuildNewHasherPipeline(
                chunkStoreDirectory is null ? new FakeChunkStore() : new LocalDirectoryChunkStore(chunkStoreDirectory),
                postageStamper,
                redundancyLevel,
                isEncrypted,
                compactionLevel);
        
        public static IHasherPipeline BuildNewHasherPipeline(
            IChunkStore chunkStore,
            IPostageStamper postageStamper,
            RedundancyLevel redundancyLevel,
            bool isEncrypted,
            int compactionLevel)
        {
            ArgumentNullException.ThrowIfNull(postageStamper, nameof(postageStamper));
            
            if (redundancyLevel != RedundancyLevel.None)
                throw new NotImplementedException();

            IHasherPipelineStage bmtStage;
            if (isEncrypted)
            {
                throw new NotImplementedException();
            }
            else
            {
                //build stages
                var shortPipelineStage = BuildNewShortHasherPipeline(chunkStore, postageStamper, compactionLevel);
                
                var chunkAggregatorStage = new ChunkAggregatorPipelineStage(
                    async (span, data) =>
                    {
                        var args = new HasherPipelineFeedArgs(span: span, data: data);
                        await shortPipelineStage.FeedAsync(args).ConfigureAwait(false);
                        return args.Hash!.Value;
                    }
                );
                var storeWriterStage = new ChunkStoreWriterPipelineStage(chunkStore, postageStamper, chunkAggregatorStage);
                bmtStage = new ChunkBmtPipelineStage(compactionLevel, storeWriterStage, postageStamper.StampIssuer);
            }
            
            return new ChunkFeederPipelineStage(bmtStage);
        }
        
        public static IHasherPipelineStage BuildNewShortHasherPipeline(
            IChunkStore chunkStore,
            IPostageStamper postageStamper,
            int compactionLevel)
        {
            ArgumentNullException.ThrowIfNull(postageStamper, nameof(postageStamper));
            
            var storeWriter = new ChunkStoreWriterPipelineStage(chunkStore, postageStamper, null);
            return new ChunkBmtPipelineStage(compactionLevel, storeWriter, postageStamper.StampIssuer);
        }
    }
}