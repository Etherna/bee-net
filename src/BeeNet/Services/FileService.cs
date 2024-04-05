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
using Etherna.BeeNet.Services.Pipelines;
using Etherna.BeeNet.Services.Putter;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public class FileService : IFileService
    {
        /// <summary>
        /// Calculate file BMT hash and report it
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="encrypt">True if content needs to be encrypted</param>
        /// <param name="redundancyLevel">Level of redundancy on chunks</param>
        /// <returns>The file hash</returns>
        public Task<SwarmAddress> GetFileHashAsync(
            Stream stream,
            bool encrypt,
            RedundancyLevel redundancyLevel)
        {
            var batchId = "tmp";
            var putter = new StamperPutter(batchId); //probably putter is not required here
            
            // Build pipeline.
            return GetFileHashHelperAsync(stream, encrypt, redundancyLevel, putter);
        }

        /// <summary>
        /// Calculate file BMT hash, create a root manifest, and report it's address
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="contentType">The file content type</param>
        /// <param name="name">The file name. If null, the file BMT hash is taken</param>
        /// <param name="encrypt">True if content needs to be encrypted</param>
        /// <param name="redundancyLevel">Level of redundancy on chunks</param>
        /// <returns>The file manifest hash</returns>
        public async Task<SwarmAddress> GetFileManifestAddressAsync(
            Stream stream,
            string contentType,
            string? name,
            bool encrypt,
            RedundancyLevel redundancyLevel,
            string batchId)
        {
            var putter = new StamperPutter(batchId);

            // Get file hash.
            var fileHash = await GetFileHashHelperAsync(stream, encrypt, redundancyLevel, putter).ConfigureAwait(false);
            name ??= fileHash.ToString();
            
            // Create manifest.
            
            // Return manifest hash.
            throw new NotImplementedException();
        }
        
        // Helpers.
        private static Task<SwarmAddress> GetFileHashHelperAsync(
            Stream stream,
            bool encrypt,
            RedundancyLevel redundancyLevel,
            StamperPutter putter)
        {
            PipelineBase pipeline = encrypt ?
                new EncryptionPipeline(putter, redundancyLevel) :
                new SimplePipeline(putter, redundancyLevel);
            return pipeline.FeedAsync(stream);
        }
    }
}