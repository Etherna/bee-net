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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    internal class StoreWriterPipelineStage : PipelineStageBase
    {
        private readonly IStoragePutter putter;

        // Constructor.
        public StoreWriterPipelineStage(
            IStoragePutter putter,
            PipelineStageBase? nextStage)
            : base(nextStage)
        {
            this.putter = putter;
        }

        // Methods.
        protected override async Task FeedImplAsync(PipelineFeedArgs args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));
            if (args.Address is null) throw new InvalidOperationException();

            putter.Put(new SwarmChunk(args.Address.Value, args.Data.ToArray()));

            await FeedNextAsync(args).ConfigureAwait(false);
        }
    }
}