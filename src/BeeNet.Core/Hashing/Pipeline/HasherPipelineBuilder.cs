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
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Hashing.Signer;
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
            IPostageStamper postageStamper,
            RedundancyLevel redundancyLevel,
            bool isEncrypted,
            ushort compactLevel,
            int? chunkConcurrency,
            bool readOnly = false)
        {
            ArgumentNullException.ThrowIfNull(postageStamper, nameof(postageStamper));

            //build stages
            var chunkAggregatorStage = new ChunkAggregatorPipelineStage(
                new ChunkParityGenerator(
                    redundancyLevel,
                    isEncrypted || compactLevel > 0,
                    new ChunkBmtPipelineStage(
                        0, //parities don't have an encryption key, so don't support chunks compaction (sig...)
                        false,
                        new ChunkStoreWriterPipelineStage(
                            chunkStore,
                            postageStamper,
                            null,
                            readOnly))),
                new ChunkReplicator(
                    redundancyLevel,
                    chunkStore,
                    postageStamper,
                    new PrivateKeySigner(SwarmSoc.ReplicasOwnerPrivateKey)),
                new ChunkBmtPipelineStage(
                    compactLevel,
                    isEncrypted,
                    new ChunkStoreWriterPipelineStage(
                        chunkStore,
                        postageStamper,
                        null,
                        readOnly)),
                readOnly);
                
            var storeWriterStage = new ChunkStoreWriterPipelineStage(
                chunkStore,
                postageStamper,
                chunkAggregatorStage,
                readOnly);
                
            var bmtStage = new ChunkBmtPipelineStage(compactLevel, isEncrypted, storeWriterStage);
            
            return new ChunkFeederPipelineStage(bmtStage, chunkConcurrency);
        }
    }
}