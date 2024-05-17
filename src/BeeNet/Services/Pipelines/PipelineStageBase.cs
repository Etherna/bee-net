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

using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services.Pipelines
{
    public abstract class PipelineStageBase
    {
        // Fields.
        protected readonly PipelineStageBase? Next;
        
        // Constructor.
        protected PipelineStageBase(PipelineStageBase? next)
        {
            Next = next;
        }

        // Methods.
        /// <summary>
        /// Flush the pipeline and perform the final sum 
        /// </summary>
        /// <returns>The binary digest of sum</returns>
        public virtual async Task<byte[]> SumAsync()
        {
            if (Next is null)
                return Array.Empty<byte>();
            return await Next.SumAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Process data into the pipeline
        /// </summary>
        /// <param name="context">The data context to process</param>
        /// <returns>Amount of processed bytes</returns>
        public virtual async Task<int> WriteAsync(PipelineWriteContext context)
        {
            if (Next is null)
                return 0;
            return await Next.WriteAsync(context).ConfigureAwait(false);
        }
    }
}