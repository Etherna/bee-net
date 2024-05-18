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
using Etherna.BeeNet.Services.Putter;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    internal abstract class PipelineBase
    {
        // Fields.
        protected readonly ChunkFeederPipelineStage chunkFeeder;
        
        // Constructor.
        protected PipelineBase(
            IPutter putter,
            RedundancyLevel redundancyLevel,
            PipelineStageBase startStage)
        {
            chunkFeeder = new ChunkFeederPipelineStage(startStage);
            Putter = putter;
            RedundancyLevel = redundancyLevel;
        }

        // Properties.
        public IPutter Putter { get; }
        public RedundancyLevel RedundancyLevel { get; }
        
        // Methods.
        /// <summary>
        /// Consume a byte array and returns a Swarm address as result
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Resulting swarm address</returns>
        public async Task<SwarmAddress> FeedAsync(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            await chunkFeeder.FeedAsync(new PipelineFeedArgs(data)).ConfigureAwait(false);
            var sum = await chunkFeeder.SumAsync().ConfigureAwait(false);
            return new SwarmAddress(sum);
        }
        
        /// <summary>
        /// Consume a stream slicing it in chunk size parts, and returns a Swarm address as result
        /// </summary>
        /// <param name="dataStream">Input data stream</param>
        /// <returns>Resulting swarm address</returns>
        public async Task<SwarmAddress> FeedAsync(Stream dataStream)
        {
            ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));
            
            // Slicing the stream permits to avoid to load all the stream in memory at the same time.
            var chunkData = new byte[SwarmChunk.Size];
            int chunkReadBytes;
            do
            {
                chunkReadBytes = await dataStream.ReadAsync(chunkData).ConfigureAwait(false);
                if (chunkReadBytes > 0)
                    await chunkFeeder.FeedAsync(new PipelineFeedArgs(chunkData[..chunkReadBytes])).ConfigureAwait(false);
            } while (chunkReadBytes == SwarmChunk.Size);

            var sum = await chunkFeeder.SumAsync().ConfigureAwait(false);
            return new SwarmAddress(sum);
        }
    }
}