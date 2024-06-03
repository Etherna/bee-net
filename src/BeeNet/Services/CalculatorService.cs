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

using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Hasher.Postage;
using Etherna.BeeNet.Hasher.Signer;
using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public class CalculatorService : ICalculatorService
    {
        public async Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            Stream stream,
            string contentType,
            string? name,
            bool encrypt,
            RedundancyLevel redundancyLevel,
            IPostageStampIssuer? postageStampIssuer = null)
        {
            postageStampIssuer ??=
                new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            
            using var hasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                new PostageStamper(
                    new FakeSigner(),
                    postageStampIssuer,
                    new MemoryStore()),
                redundancyLevel,
                encrypt);
            
            // Get file hash.
            var fileAddress = await hasherPipeline.HashDataAsync(stream).ConfigureAwait(false);
            
            // Create manifest.
            name ??= fileAddress.ToString();
            var manifestAddress = fileAddress;
            
            // Return result.
            return new UploadEvaluationResult(
                manifestAddress,
                postageStampIssuer);
        }
    }
}