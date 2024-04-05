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

namespace Etherna.BeeNet.Services.Pipelines
{
    public class SimplePipeline : PipelineBase
    {
        // Constructor.
        public SimplePipeline(
            StamperPutter putter,
            RedundancyLevel redundancyLevel)
            : base(putter, redundancyLevel)
        {
            //build stages
            var shortPipelineStage = ShortPipelineStage.BuildNewStage(putter);
            var hashTrieWriterStage = hashtrie.NewHashTrieWriter(
                SwarmAddress.HashSize,
                redundancy.New(redundancyLevel, false, shortPipelineStage),
                shortPipelineStage,
                putter
            );
            var storeWriterStage = store.NewStoreWriter(putter, hashTrieWriterStage);
            var bmtWriterStage = bmt.NewBmtWriter(storeWriterStage);
            StartStage = feeder.NewChunkFeederWriter(SwarmChunk.Size, bmtWriterStage);
        }

        // Properties.
        public override PipelineStageBase StartStage { get; }

        // Methods.
        public override int Write(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Sum()
        {
            throw new System.NotImplementedException();
        }
    }
}