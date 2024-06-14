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
            byte[] data,
            string fileContentType,
            string? fileName,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            string? chunkStoreDirectory = null)
        {
            using var stream = new MemoryStream(data);
            return await EvaluateFileUploadAsync(
                stream,
                fileContentType,
                fileName,
                encrypt,
                redundancyLevel,
                postageStampIssuer,
                chunkStoreDirectory).ConfigureAwait(false);
        }

        public async Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
            bool encrypt,
            RedundancyLevel redundancyLevel,
            IPostageStampIssuer? postageStampIssuer = null,
            string? chunkStoreDirectory = null)
        {
            postageStampIssuer ??= new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            var postageStamper = new PostageStamper(
                new FakeSigner(),
                postageStampIssuer,
                new MemoryStampStore());
            
            // Get file hash.
            using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                postageStamper,
                redundancyLevel,
                encrypt,
                chunkStoreDirectory);
            var fileHash = await fileHasherPipeline.HashDataAsync(stream).ConfigureAwait(false);
            fileName ??= fileHash.ToString(); //if missing, set file name with its address
            
            // Create manifest.
            var manifest = new MantarayManifest(
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    postageStamper,
                    redundancyLevel,
                    encrypt,
                    chunkStoreDirectory),
                encrypt);

            manifest.Add(
                MantarayManifest.RootPath,
                ManifestEntry.NewDirectory(
                    new Dictionary<string, string>
                    {
                        [ManifestEntry.WebsiteIndexDocPathKey] = fileName,
                    }));
            
            manifest.Add(
                fileName,
                ManifestEntry.NewFile(
                    fileHash,
                    new Dictionary<string, string>
                    {
                        [ManifestEntry.ContentTypeKey] = fileContentType,
                        [ManifestEntry.FilenameKey] = fileName
                    }));

            var finalHash = await manifest.GetHashAsync().ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                finalHash,
                postageStampIssuer);
        }

        public async Task<Stream> GetResourceStreamFromChunks(
            string chunkStoreDirectory,
            SwarmAddress address)
        {
            var chunkStore = new LocalDirectoryChunkStore(chunkStoreDirectory);

            var rootManifest = await MantarayManifest.CreateFromStoredChunkAsync(
                chunkStore,
                address.Hash,
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    new FakePostageStamper(),
                    RedundancyLevel.None,
                    false,
                    chunkStoreDirectory)).ConfigureAwait(false);

            return await rootManifest.GetResourceStreamAsync(address).ConfigureAwait(false);
        }

        public Task<IEnumerable<string>> GetResourceListFromChunks(string chunkStoreDirectory, SwarmAddress address)
        {
            throw new System.NotImplementedException();
        }
    }
}