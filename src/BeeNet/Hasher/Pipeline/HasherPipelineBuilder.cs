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
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Hasher.Pipeline
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    internal static class HasherPipelineBuilder
    {
        // Static builders.
        public static IHasherPipeline BuildNewHasherPipeline(
            IPostageStamper postageStamper,
            RedundancyLevel redundancyLevel,
            bool isEncrypted)
        {
            if (redundancyLevel != RedundancyLevel.None)
                throw new NotImplementedException();

            IHasherPipelineStage bmtStage;
            if (isEncrypted)
            {
                throw new NotImplementedException();
            }
            else
            {
                //build stages
                var shortPipelineStage = BuildNewShortHasherPipeline(postageStamper);
                
                var chunkAggregatorStage = new ChunkAggregatorPipelineStage(
                    async (span, data) =>
                    {
                        var args = new HasherPipelineFeedArgs(span: span, data: data);
                        await shortPipelineStage.FeedAsync(args).ConfigureAwait(false);
                        return args.Address!.Value;
                    }
                );
                var storeWriterStage = new ChunkStoreWriterPipelineStage(postageStamper, chunkAggregatorStage);
                bmtStage = new ChunkBmtPipelineStage(storeWriterStage);
            }
            
            return new ChunkFeederPipelineStage(bmtStage);
        }
        
        public static IHasherPipelineStage BuildNewShortHasherPipeline(IPostageStamper postageStamper)
        {
            var storeWriter = new ChunkStoreWriterPipelineStage(postageStamper, null);
            return new ChunkBmtPipelineStage(storeWriter);
        }
    }
}