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
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    internal sealed class ChunkStoreWriterPipelineStage : PipelineStageBase
    {
        private readonly IPostageStamper postageStamper;

        // Constructor.
        public ChunkStoreWriterPipelineStage(
            IPostageStamper postageStamper,
            PipelineStageBase? nextStage)
            : base(nextStage)
        {
            this.postageStamper = postageStamper;
        }

        // Methods.
        protected override async Task FeedImplAsync(PipelineFeedArgs args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));
            if (args.Address is null) throw new InvalidOperationException();

            postageStamper.Stamp(args.Address.Value);

            await FeedNextAsync(args).ConfigureAwait(false);
        }
    }
}