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

using Etherna.BeeNet.Hasher.Postage;
using Etherna.BeeNet.Models;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Pipeline
{
    internal sealed class ChunkStoreWriterPipelineStage : IHasherPipelineStage
    {
        // Fields.
        private readonly IHasherPipelineStage? nextStage;
        private readonly IPostageStamper postageStamper;

        // Constructor.
        public ChunkStoreWriterPipelineStage(
            IPostageStamper postageStamper,
            IHasherPipelineStage? nextStage)
        {
            this.nextStage = nextStage;
            this.postageStamper = postageStamper;
        }
        
        // Dispose.
        public void Dispose()
        {
            nextStage?.Dispose();
        }

        // Methods.
        public async Task FeedAsync(HasherPipelineFeedArgs args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));
            if (args.Address is null) throw new InvalidOperationException();

            // Stamp chunk and store stamp.
            postageStamper.Stamp(args.Address.Value);
            
            // Store chunk.
            //not implemented...

            if (nextStage is not null)
                await nextStage.FeedAsync(args).ConfigureAwait(false);
        }
        
        public Task<SwarmAddress> SumAsync() =>
            nextStage?.SumAsync() ?? throw new InvalidOperationException();
    }
}