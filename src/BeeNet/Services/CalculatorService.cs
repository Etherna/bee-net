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
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public class CalculatorService : ICalculatorService
    {
        public async Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
            bool encrypt,
            RedundancyLevel redundancyLevel,
            IPostageStampIssuer? postageStampIssuer = null)
        {
            postageStampIssuer ??= new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            var postageStamper = new PostageStamper(
                new FakeSigner(),
                postageStampIssuer,
                new MemoryStore());
            
            // Get file hash.
            using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                postageStamper,
                redundancyLevel,
                encrypt);
            var fileAddress = await fileHasherPipeline.HashDataAsync(stream).ConfigureAwait(false);
            
            // Create manifest.
            fileName ??= fileAddress.ToString();
            using var manifestHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                postageStamper,
                redundancyLevel,
                encrypt);
            var manifest = new MantarayManifest(manifestHasherPipeline, encrypt);

            var rootMetadata = new Dictionary<string, string>()
            {
                [MantarayManifest.WebsiteIndexDocumentSuffixKey] = fileName,
            };

            manifest.Add(MantarayManifest.RootPath, new ManifestEntry(SwarmAddress.Zero, rootMetadata));

            var fileMtdt = new Dictionary<string, string>
            {
                [MantarayManifest.EntryMetadataContentTypeKey] = fileContentType,
                [MantarayManifest.EntryMetadataFilenameKey] = fileName
            };
            
            manifest.Add(fileName, new ManifestEntry(fileAddress, fileMtdt));

            var manifestAddress = await manifest.StoreAsync().ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                manifestAddress,
                postageStampIssuer);
        }
    }
}