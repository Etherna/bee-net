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

namespace Etherna.BeeNet.Pipelines
{
    internal class DefaultPipeline : PipelineBase
    {
        // Constructor.
        protected DefaultPipeline(
            IPutter putter,
            RedundancyLevel redundancyLevel,
            PipelineStageBase startStage)
            : base(putter, redundancyLevel, startStage)
        { }

        // Factory methods.
        public static DefaultPipeline BuildPipeline(
            IPutter putter,
            RedundancyLevel redundancyLevel)
        {
            //build stages
            var shortPipelineStage = ShortPipelineStage.BuildNewStage(putter);
            var hashTrieWriterStage = new HashTrieWriterPipelineStage(
                SwarmAddress.HashByteSize,
                new RedundancyParams(redundancyLevel, false, shortPipelineStage),
                shortPipelineStage,
                putter
            );
            var storeWriterStage = new StoreWriterPipelineStage(putter, hashTrieWriterStage);
            var bmtWriterStage = new BmtWriterPipelineStage(storeWriterStage);

            return new DefaultPipeline(putter, redundancyLevel, bmtWriterStage);
        }
    }
}