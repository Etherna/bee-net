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
using Etherna.BeeNet.Models.Bmt;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    internal class BmtWriterPipelineStage : PipelineStageBase
    {
        public BmtWriterPipelineStage(PipelineStageBase? nextStage)
            : base(nextStage)
        { }

        public override async Task FeedAsync(PipelineFeedArgs args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));
            if (args.Data.Length < SwarmChunk.SpanSize)
                throw new InvalidOperationException();

            var data = args.Data.ToArray();
            if (!BmtPool.Instance.TryGet(out var hasher))
                throw new NotImplementedException(); //try to not use a pool
            hasher!.SetHeader(data[..SwarmChunk.SpanSize]);
            hasher.Write(data[SwarmChunk.SpanSize..]);
            args.Reference = hasher.Hash(null);
            BmtPool.Instance.Put(hasher);

            await FeedNextAsync(args).ConfigureAwait(false);
        }
    }
}