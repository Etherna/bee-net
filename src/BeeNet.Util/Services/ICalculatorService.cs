// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Hashing.Store;
using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public interface ICalculatorService
    {
        /// <summary>
        /// Evaluate the result uploading a directory
        /// </summary>
        /// <param name="directoryPath">The directory to upload</param>
        /// <param name="indexFilename">The index default file</param>
        /// <param name="errorFilename">The error default file</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> EvaluateDirectoryUploadAsync(
            string directoryPath,
            string? indexFilename = null,
            string? errorFilename = null,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null);
        
        /// <summary>
        /// Evaluate the result uploading a file
        /// </summary>
        /// <param name="data">The file data in byte array</param>
        /// <param name="fileContentType">The file content type</param>
        /// <param name="fileName">The file name</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            byte[] data,
            string fileContentType,
            string? fileName,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null);
        
        /// <summary>
        /// Evaluate the result uploading a file
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="fileContentType">The file content type</param>
        /// <param name="fileName">The file name</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
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