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

using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public interface IChunkService
    {
        /// <summary>
        /// Get resource metadata from a directory of chunks and the resource address
        /// </summary>
        /// <param name="address">Resource address</param>
        /// <param name="chunkStore">The chunk reading store</param>
        /// <returns>Resource metadata</returns>
        Task<IReadOnlyDictionary<string, string>> GetFileMetadataFromChunksAsync(
            SwarmAddress address,
            IReadOnlyChunkStore chunkStore);
        
        /// <summary>
        /// Get resource stream from a chunks store and the resource address
        /// </summary>
        /// <param name="chunkStore">The chunk store</param>
        /// <param name="address">Resource address</param>
        /// <param name="fileCachePath">Optional file where store read data. Necessary if data is >2GB</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Resource stream</returns>
        Task<Stream> GetFileStreamFromChunksAsync(
            IChunkStore chunkStore,
            SwarmAddress address,
            string? fileCachePath = null,
            CancellationToken? cancellationToken = null);
    
        /// <summary>
        /// Evaluate the result uploading a directory
        /// </summary>
        /// <param name="directoryPath">The directory to upload</param>
        /// <param name="indexFilename">The index default file</param>
        /// <param name="errorFilename">The error default file</param>
        /// <param name="compactLevel">Chunk compact level [0, 65535]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStamper">Custom postage stamper</param>
        /// <param name="chunkCuncorrency">Amount of concurrent chunk hashing tasks. Null to default</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> UploadDirectoryAsync(
            string directoryPath,
            Func<IHasher> hasherBuilder,
            string? indexFilename = null,
            string? errorFilename = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null,
            IChunkStore? chunkStore = null);

        /// <summary>
        /// Evaluate the result uploading a directory
        /// </summary>
        /// <param name="fileNames">The list of files names</param>
        /// <param name="getFileStream">Return a file stream from its name</param>
        /// <param name="indexFilename">The index default file</param>
        /// <param name="errorFilename">The error default file</param>
        /// <param name="compactLevel">Chunk compact level [0, 65535]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStamper">Custom postage stamper</param>
        /// <param name="chunkCuncorrency">Amount of concurrent chunk hashing tasks. Null to default</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> UploadDirectoryAsync(
            string[] fileNames,
            Func<string, Stream> getFileStream,
            Func<IHasher> hasherBuilder,
            string? indexFilename = null,
            string? errorFilename = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null,
            IChunkStore? chunkStore = null);
        
        /// <summary>
        /// Evaluate the result uploading a single file
        /// </summary>
        /// <param name="data">The file data in byte array</param>
        /// <param name="fileContentType">The file content type</param>
        /// <param name="fileName">The file name</param>
        /// <param name="compactLevel">Chunk compact level [0, 65535]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStamper">Custom postage stamper</param>
        /// <param name="chunkCuncorrency">Amount of concurrent chunk hashing tasks. Null to default</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> UploadSingleFileAsync(
            byte[] data,
            string fileContentType,
            string? fileName,
            Func<IHasher> hasherBuilder,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null,
            IChunkStore? chunkStore = null);
        
        /// <summary>
        /// Evaluate the result uploading a single file
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="fileContentType">The file content type</param>
        /// <param name="fileName">The file name</param>
        /// <param name="compactLevel">Chunk compact level [0, 65535]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStamper">Custom postage stamper</param>
        /// <param name="chunkCuncorrency">Amount of concurrent chunk hashing tasks. Null to default</param>
        /// <param name="chunkStore">Optional custom chunk store</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> UploadSingleFileAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
            Func<IHasher> hasherBuilder,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null,
            IChunkStore? chunkStore = null);
        
        /// <summary>
        /// Write data chunks on a local directory, without any manifest
        /// </summary>
        /// <param name="chunkStore">The chunk store</param>
        /// <param name="data">The data byte array input</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <param name="compactLevel">Chunk compact level [0, 65535]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="chunkCuncorrency">Amount of concurrent chunk hashing tasks. Null to default</param>
        /// <returns>The data root hash</returns>
        Task<SwarmHash> WriteDataChunksAsync(
            IChunkStore chunkStore,
            byte[] data,
            Func<IHasher> hasherBuilder,
            IPostageStampIssuer? postageStampIssuer = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            int? chunkCuncorrency = null);
        
        /// <summary>
        /// Write data chunks on a local directory, without any manifest
        /// </summary>
        /// <param name="chunkStore">The chunk store</param>
        /// <param name="stream">The data stream input</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <param name="compactLevel">Chunk compact level [0, 65535]</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="chunkCuncorrency">Amount of concurrent chunk hashing tasks. Null to default</param>
        /// <returns>The data root hash</returns>
        Task<SwarmHash> WriteDataChunksAsync(
            IChunkStore chunkStore,
            Stream stream,
            Func<IHasher> hasherBuilder,
            IPostageStampIssuer? postageStampIssuer = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            int? chunkCuncorrency = null);
    }
}