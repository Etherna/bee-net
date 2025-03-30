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
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    public static class HasherPipelineBuilder
    {
        // Static builders.
        public static IHasherPipeline BuildNewHasherPipeline(
            IChunkStore chunkStore,
            Func<IHasher> hasherBuilder,
            IPostageStamper postageStamper,
            RedundancyLevel redundancyLevel,
            bool isEncrypted,
            ushort compactLevel,
            int? chunkConcurrency,
            bool readOnly = false)
        {
            ArgumentNullException.ThrowIfNull(hasherBuilder, nameof(hasherBuilder));
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
                var chunkAggregatorStage = new ChunkAggregatorPipelineStage(
                    new ChunkBmtPipelineStage(
                        compactLevel,
                        new ChunkStoreWriterPipelineStage(
                            chunkStore,
                            postageStamper,
                            null,
                            readOnly)),
                    compactLevel > 0);
                
                var storeWriterStage = new ChunkStoreWriterPipelineStage(
                    chunkStore,
                    postageStamper,
                    chunkAggregatorStage,
                    readOnly);
                
                bmtStage = new ChunkBmtPipelineStage(compactLevel, storeWriterStage);
            }
            
            return new ChunkFeederPipelineStage(bmtStage, hasherBuilder, chunkConcurrency);
        }
    }
}