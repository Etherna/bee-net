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
        // Constructor.
        protected PipelineBase(
            StamperPutter putter,
            RedundancyLevel redundancyLevel)
        {
            Putter = putter;
            RedundancyLevel = redundancyLevel;
        }

        // Properties.
        public StamperPutter Putter { get; }
        public RedundancyLevel RedundancyLevel { get; }
        public abstract PipelineStageBase StartStage { get; }
        
        // Methods.
        public async Task<SwarmAddress> FeedAsync(Stream fileStream)
        {
            ArgumentNullException.ThrowIfNull(fileStream, nameof(fileStream));
            
            var chunkData = new byte[SwarmChunk.Size];
            int chunkReadBytes;
            do
            {
                chunkReadBytes = await fileStream.ReadAsync(chunkData).ConfigureAwait(false);
                if (chunkReadBytes > 0)
                    Write(chunkData[..chunkReadBytes]);
            } while (chunkReadBytes == SwarmChunk.Size);

            var sum = Sum();
            return new SwarmAddress(sum);
        }
        
        public abstract int Write(byte[] bytes);
        public abstract byte[] Sum();
    }
}