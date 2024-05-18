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

namespace Etherna.BeeNet.Pipelines
{
    internal abstract class PipelineStageBase
    {
        // Fields.
        private readonly PipelineStageBase? nextStage;
        
        // Constructor.
        protected PipelineStageBase(PipelineStageBase? nextStage)
        {
            this.nextStage = nextStage;
        }

        // Methods.
        /// <summary>
        /// Process data into the pipeline stage
        /// </summary>
        /// <param name="args">The pipeline stage feed arguments</param>
        public virtual Task FeedAsync(PipelineFeedArgs args) => FeedNextAsync(args);

        /// <summary>
        /// Flush the pipeline and perform the final sum 
        /// </summary>
        /// <returns>The binary digest of sum</returns>
        public virtual Task<byte[]> SumAsync() => SumNextAsync();
        
        // Protected methods.
        protected Task FeedNextAsync(PipelineFeedArgs args) =>
            nextStage is not null ? nextStage.FeedAsync(args) : Task.CompletedTask;

        protected Task<byte[]> SumNextAsync() =>
            nextStage is not null ? nextStage.SumAsync() : Task.FromResult(Array.Empty<byte>());
    }
}