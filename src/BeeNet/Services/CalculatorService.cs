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
using Etherna.BeeNet.Pipelines;
using Etherna.BeeNet.Postage;
using Etherna.BeeNet.Signer;
using Etherna.BeeNet.Store;
using Nethereum.Util;
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
                new PostageStampIssuer(
                    PostageBatch.MaxDepthInstance,
                    AddressUtil.ZERO_ADDRESS);
            
            var hasherPipeline = new HasherPipeline(
                new PostageStamper(
                    postageStampIssuer,
                    new FakeSigner(),
                    new MemoryStore()),
                redundancyLevel,
                encrypt);
            
            // Get file hash.
            var fileAddress = await hasherPipeline.FeedAsync(stream).ConfigureAwait(false);
            
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