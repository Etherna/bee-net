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

namespace Etherna.BeeNet.Services.Pipelines
{
    public class StoreWriterPipelineStage : PipelineStageBase
    {
        private readonly StamperPutter putter;

        // Constructor.
        public StoreWriterPipelineStage(
            StamperPutter putter,
            PipelineStageBase? next)
            : base(next)
        {
            this.putter = putter;
        }

        // Methods.
        public override void ChainWrite(PipelineWriteContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            if (context.Reference is null || context.Data is null)
                throw new InvalidOperationException();

            putter.Put(new SwarmChunk(new SwarmAddress(context.Reference), context.Data));
            Next?.ChainWrite(context);
        }
    }
}