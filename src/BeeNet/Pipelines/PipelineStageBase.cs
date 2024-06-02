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
    internal abstract class PipelineStageBase : IDisposable
    {
        // Fields.
        private readonly PipelineStageBase? nextStage;
        
        // Constructor.
        protected PipelineStageBase(PipelineStageBase? nextStage)
        {
            this.nextStage = nextStage;
        }
        
        // Dispose.
        public virtual void Dispose()
        {
            nextStage?.Dispose();
        }
        
        // Properties.
        public bool IsClosed { get; private set; }

        // Methods.
        /// <summary>
        /// Process data into the pipeline stage
        /// </summary>
        /// <param name="args">The pipeline stage feed arguments</param>
        public Task FeedAsync(PipelineFeedArgs args)
        {
            if (IsClosed)
                throw new InvalidOperationException("Can't feed other data if stage is closed");
            return FeedImplAsync(args);
        }

        /// <summary>
        /// Flush the pipeline and perform the final sum 
        /// </summary>
        /// <returns>The binary digest of sum</returns>
        public async Task<SwarmAddress> SumAsync()
        {
            var sum = await SumImplAsync().ConfigureAwait(false);
            IsClosed = true;
            return sum;
        }
        
        // Protected virtual methods.
        /// <summary>
        /// Virtual implementation of the feed method
        /// </summary>
        /// <param name="args">The pipeline stage feed arguments</param>
        protected virtual Task FeedImplAsync(PipelineFeedArgs args) => FeedNextAsync(args);

        /// <summary>
        /// Virtual implementation of the sum method
        /// </summary>
        /// <returns>The binary digest of sum</returns>
        protected virtual Task<SwarmAddress> SumImplAsync() => SumNextAsync();
        
        // Protected helper methods.
        protected Task FeedNextAsync(PipelineFeedArgs args) =>
            nextStage?.FeedAsync(args) ?? Task.CompletedTask;

        protected Task<SwarmAddress> SumNextAsync() =>
            nextStage?.SumAsync() ?? throw new InvalidOperationException();
    }
}