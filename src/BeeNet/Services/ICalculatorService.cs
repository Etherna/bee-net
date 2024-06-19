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
using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public interface ICalculatorService
    {
        /// <summary>
        /// Evaluate the result of upload a file
        /// </summary>
        /// <param name="data">The file data in byte array</param>
        /// <param name="fileContentType">The file content type</param>
        /// <param name="fileName">The file name</param>
        /// <param name="compactionLevel">Value of required postage buckets compaction, in range [0, 100]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            byte[] data,
            string fileContentType,
            string? fileName,
            int compactionLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null);
        
        /// <summary>
        /// Evaluate the result of upload a file
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="fileContentType">The file content type</param>
        /// <param name="fileName">The file name</param>
        /// <param name="compactionLevel">Value of required postage buckets compaction, in range [0, 100]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
            int compactionLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null);

        /// <summary>
        /// Get resource metadata from a directory of chunks and the resource address
        /// </summary>
        /// <param name="chunkStoreDirectory">The chunk directory</param>
        /// <param name="address">Resource address</param>
        /// <returns>Resource metadata</returns>
        Task<IReadOnlyDictionary<string, string>> GetResourceMetadataFromChunksAsync(
            string chunkStoreDirectory,
            SwarmAddress address);
        
        /// <summary>
        /// Get resource stream from a directory of chunks and the resource address
        /// </summary>
        /// <param name="chunkStoreDirectory">The chunk directory</param>
        /// <param name="address">Resource address</param>
        /// <returns>Resource stream</returns>
        Task<Stream> GetResourceStreamFromChunksAsync(
            string chunkStoreDirectory,
            SwarmAddress address);
    }
}