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

namespace Etherna.BeeNet.Services.Pipelines
{
    public abstract class PipelineBase
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
        public async Task<SwarmAddress> FeedAsync(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            for (var i = 0; i < data.Length;)
            {
                var chunkReadBytes = Math.Min(SwarmChunk.Size, data.Length - i);
                await WriteAsync(data[i..(i + chunkReadBytes)]).ConfigureAwait(false);
                i += chunkReadBytes;
            }

            var sum = await SumAsync().ConfigureAwait(false);
            return new SwarmAddress(sum);
        }
        
        public async Task<SwarmAddress> FeedAsync(Stream dataStream)
        {
            ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));
            
            var chunkData = new byte[SwarmChunk.Size];
            int chunkReadBytes;
            do
            {
                chunkReadBytes = await dataStream.ReadAsync(chunkData).ConfigureAwait(false);
                if (chunkReadBytes > 0)
                    await WriteAsync(chunkData[..chunkReadBytes]).ConfigureAwait(false);
            } while (chunkReadBytes == SwarmChunk.Size);

            var sum = await SumAsync().ConfigureAwait(false);
            return new SwarmAddress(sum);
        }
        
        // Protected methods.
        protected virtual Task<byte[]> SumAsync() => chunkFeeder.SumAsync();
        
        protected virtual Task<int> WriteAsync(byte[] bytes) => chunkFeeder.WriteAsync(new PipelineWriteContext
        {
            Data = bytes
        });
    }
}