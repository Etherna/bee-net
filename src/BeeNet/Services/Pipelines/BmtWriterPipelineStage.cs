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

namespace Etherna.BeeNet.Services.Pipelines
{
    public class BmtWriterPipelineStage : PipelineStageBase
    {
        public BmtWriterPipelineStage(PipelineStageBase? next)
            : base(next)
        { }

        public override async Task<int> WriteAsync(PipelineWriteContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            if (context.Data is null)
                throw new InvalidOperationException();
            if (context.Data.Length < SwarmChunk.SpanSize)
                throw new InvalidOperationException();

            if (!BmtPool.Instance.TryGet(out var hasher))
                throw new NotImplementedException(); //try to not use a pool
            hasher!.SetHeader(context.Data[..SwarmChunk.SpanSize]);
            hasher.Write(context.Data[SwarmChunk.SpanSize..]);
            context.Reference = hasher.Hash(null);
            BmtPool.Instance.Put(hasher);

            if (Next is null)
                return 0;
            return await Next.WriteAsync(context).ConfigureAwait(false);
        }
    }
}