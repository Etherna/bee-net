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

using Etherna.BeeNet.Postage;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Pipelines
{
    internal sealed class ShortPipelineStage(PipelineStageBase? nextStage)
        : PipelineStageBase(nextStage)
    {
        // Builders.
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
        public static ShortPipelineStage BuildNewStage(IPostageStamper postageStamper)
        {
            var storeWriter = new ChunkStoreWriterPipelineStage(postageStamper, null);
            var next = new ChunkBmtPipelineStage(storeWriter);
            return new ShortPipelineStage(next);
        }
    }
}