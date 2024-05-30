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
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    /// <summary>
    /// Calculate hash of each chunk
    /// </summary>
    internal class ChunkBmtPipelineStage : PipelineStageBase
    {
        // Constructor.
        public ChunkBmtPipelineStage(PipelineStageBase nextStage)
            : base(nextStage)
        { }

        // Protected methods.
        protected override async Task FeedImplAsync(PipelineFeedArgs args)
        {
            if (args.Data.Length < SwarmChunk.SpanSize)
                throw new InvalidOperationException("Data can't be shorter than span size here");
            if (args.Data.Length > SwarmChunk.SpanAndDataSize)
                throw new InvalidOperationException("Data can't be longer than chunk + span size here");

            args.Address = SwarmChunkBmtHasher.Hash(
                args.Data[..SwarmChunk.SpanSize].ToArray(),
                args.Data[SwarmChunk.SpanSize..].ToArray());

            await FeedNextAsync(args).ConfigureAwait(false);
        }
    }
}