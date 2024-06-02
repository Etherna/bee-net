// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Postage;
using Etherna.BeeNet.Redundancy;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    internal sealed class HasherPipeline : IDisposable
    {
        // Fields.
        private readonly ChunkFeederPipelineStage chunkFeeder;
        
        // Constructor.
        public HasherPipeline(
            IPostageStamper postageStamper,
            RedundancyLevel redundancyLevel,
            bool isEncrypted)
        {
            if (redundancyLevel != RedundancyLevel.None)
                throw new NotImplementedException();

            PipelineStageBase startStage;
            if (isEncrypted)
            {
                throw new NotImplementedException();
            }
            else
            {
                //build stages
                var shortPipelineStage = ShortPipelineStage.BuildNewStage(postageStamper);
                
                var chunkAggregatorStage = new ChunkAggregatorPipelineStage(
                    new RedundancyParams(redundancyLevel, false, shortPipelineStage),
                    postageStamper,
                    async (span, data) =>
                    {
                        var args = new PipelineFeedArgs(span: span, data: data);
                        await shortPipelineStage.FeedAsync(args).ConfigureAwait(false);
                        return args.Address!.Value;
                    }
                );
                var storeWriterStage = new ChunkStoreWriterPipelineStage(postageStamper, chunkAggregatorStage);
                startStage = new ChunkBmtPipelineStage(storeWriterStage);
            }
            
            chunkFeeder = new ChunkFeederPipelineStage(startStage);
            PostageStamper = postageStamper;
            RedundancyLevel = redundancyLevel;
        }
        
        // Dispose.
        public void Dispose()
        {
            chunkFeeder.Dispose();
        }

        // Properties.
        public IPostageStamper PostageStamper { get; }
        public RedundancyLevel RedundancyLevel { get; }
        
        // Methods.
        /// <summary>
        /// Consume a byte array and returns a Swarm address as result
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Resulting swarm address</returns>
        public Task<SwarmAddress> FeedAsync(byte[] data) => chunkFeeder.FeedAsync(data);

        /// <summary>
        /// Consume a stream slicing it in chunk size parts, and returns a Swarm address as result
        /// </summary>
        /// <param name="dataStream">Input data stream</param>
        /// <returns>Resulting swarm address</returns>
        public Task<SwarmAddress> FeedAsync(Stream dataStream) => chunkFeeder.FeedAsync(dataStream);
    }
}