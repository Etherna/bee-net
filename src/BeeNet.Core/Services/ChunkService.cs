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

using Etherna.BeeNet.Chunks;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Hashing.Signer;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public class ChunkService : IChunkService
    {
        // Methods.
        public async Task<Stream> GetFileStreamFromAddressAsync(
            SwarmAddress address,
            ManifestPathResolver manifestPathResolver,
            IReadOnlyChunkStore chunkStore,
            string? fileCachePath = null,
            CancellationToken? cancellationToken = null)
        {
            var chunkJoiner = new ChunkJoiner(chunkStore);

            var chunkReference = (await address.ResolveToResourceInfoAsync(
                chunkStore, manifestPathResolver).ConfigureAwait(false)).Result.ChunkReference;
            
            return await chunkJoiner.GetJoinedChunkDataAsync(
                chunkReference,
                fileCachePath,
                cancellationToken).ConfigureAwait(false);
        }

        public Task<UploadEvaluationResult> UploadDirectoryAsync(
            string directoryPath,
            Hasher hasher,
            string? indexFilename = null,
            string? errorFilename = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null, 
            IChunkStore? chunkStore = null)
        {
            // Get all files from directory.
            var files = Directory.GetFiles(directoryPath, "", SearchOption.AllDirectories);

            // Evaluate upload.
            return UploadDirectoryAsync(
                files.Select(f => Path.GetRelativePath(directoryPath, f)).ToArray(),
                f =>  File.OpenRead(Path.Combine(directoryPath, f)),
                hasher,
                indexFilename,
                errorFilename,
                compactLevel,
                encrypt,
                redundancyLevel,
                postageStamper,
                chunkCuncorrency,
                chunkStore);
        }

        public async Task<UploadEvaluationResult> UploadDirectoryAsync(
            string[] fileNames,
            Func<string, Stream> getFileStream,
            Hasher hasher,
            string? indexFilename = null,
            string? errorFilename = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null,
            IChunkStore? chunkStore = null)
        {
            // Checks.
            ArgumentNullException.ThrowIfNull(fileNames, nameof(fileNames));
            ArgumentNullException.ThrowIfNull(getFileStream, nameof(getFileStream));
            
            if (fileNames.Length == 0)
                throw new ArgumentException("No files in directory", nameof(fileNames));
            if (fileNames.Any(f => f.StartsWith(SwarmAddress.Separator)))
                throw new ArgumentException(
                    "File names can't start with slash character",
                    nameof(fileNames));
            if (indexFilename?.Contains(SwarmAddress.Separator, StringComparison.InvariantCulture) == true)
                throw new ArgumentException(
                    "Index document suffix must not include slash character",
                    nameof(indexFilename));
            if (errorFilename?.Contains(SwarmAddress.Separator, StringComparison.InvariantCulture) == true)
                throw new ArgumentException(
                    "Error document suffix must not include slash character",
                    nameof(errorFilename));
            
            // Init.
            chunkStore ??= new FakeChunkStore();
            postageStamper ??= new PostageStamper(
                new FakeSigner(),
                new PostageStampIssuer(PostageBatch.MaxDepthInstance),
                new MemoryStampStore());
            long totalMissedOptimisticHashing = 0;
            
            // Try set index document.
            if (indexFilename is null && fileNames.Contains(SwarmHttpConsts.DefaultIndexFilename))
                indexFilename = SwarmHttpConsts.DefaultIndexFilename;
            
            // Create manifest.
            var dirManifest = new MantarayManifest(
                readOnlyPipeline => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    redundancyLevel,
                    encrypt,
                    0,
                    chunkCuncorrency,
                    readOnlyPipeline),
                compactLevel);

            // Iterate through the files.
            foreach (var file in fileNames)
            {
                using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    redundancyLevel,
                    encrypt,
                    compactLevel,
                    chunkCuncorrency);
                
                var fileContentType = FileContentTypeProvider.GetContentType(file);
                var fileName = Path.GetFileName(file);
                var fileStream = getFileStream(file);
                await using (fileStream.ConfigureAwait(false))
                {
                    var fileHashingResult = await fileHasherPipeline.HashDataAsync(fileStream).ConfigureAwait(false);
                    totalMissedOptimisticHashing += fileHasherPipeline.MissedOptimisticHashing;
                
                    // Add file entry to dir manifest.
                    var fileEntryMetadata = new Dictionary<string, string>
                    {
                        [ManifestEntry.ContentTypeKey] = fileContentType,
                        [ManifestEntry.FilenameKey] = fileName
                    };
                    if (fileHashingResult.EncryptionKey != null)
                        fileEntryMetadata.Add(ManifestEntry.ChunkEncryptKeyKey, fileHashingResult.EncryptionKey.Value.ToString());
                    if (fileHashingResult.UseRecursiveEncryption)
                        fileEntryMetadata.Add(ManifestEntry.UseRecursiveEncryptionKey, true.ToString());
                
                    dirManifest.Add(
                        file,
                        ManifestEntry.NewFile(
                            fileHashingResult.Hash,
                            fileEntryMetadata));
                }
            }
            
            // Store website information.
            if (!string.IsNullOrEmpty(indexFilename) ||
                !string.IsNullOrEmpty(errorFilename))
            {
                var metadata = new Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(indexFilename))
                    metadata[ManifestEntry.WebsiteIndexDocPathKey] = indexFilename;
                if (!string.IsNullOrEmpty(errorFilename))
                    metadata[ManifestEntry.WebsiteErrorDocPathKey] = errorFilename;

                var rootManifestEntry = ManifestEntry.NewDirectory(metadata);
                dirManifest.Add(MantarayManifest.RootPath, rootManifestEntry);
            }

            // Get manifest hash.
            var chunkHashingResult = await dirManifest.GetHashAsync(hasher).ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                chunkHashingResult,
                totalMissedOptimisticHashing,
                postageStamper.StampIssuer);
        }

        public async Task<UploadEvaluationResult> UploadSingleFileAsync(
            byte[] data,
            string fileContentType,
            string? fileName,
            Hasher hasher,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null, 
            IChunkStore? chunkStore = null)
        {
            using var stream = new MemoryStream(data);
            return await UploadSingleFileAsync(
                stream,
                fileContentType,
                fileName,
                hasher,
                compactLevel,
                encrypt,
                redundancyLevel,
                postageStamper,
                chunkCuncorrency,
                chunkStore).ConfigureAwait(false);
        }

        public async Task<UploadEvaluationResult> UploadSingleFileAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
            Hasher hasher,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStamper? postageStamper = null,
            int? chunkCuncorrency = null, 
            IChunkStore? chunkStore = null)
        {
            // Checks.
            if (fileName?.Contains(SwarmAddress.Separator, StringComparison.InvariantCulture) == true)
                throw new ArgumentException(
                    "File name must not include slash character",
                    nameof(fileName));

            // Init.
            chunkStore ??= new FakeChunkStore();
            postageStamper ??= new PostageStamper(
                new FakeSigner(),
                new PostageStampIssuer(PostageBatch.MaxDepthInstance),
                new MemoryStampStore());
            
            // Get file hash.
            using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                chunkStore,
                postageStamper,
                redundancyLevel,
                encrypt,
                compactLevel,
                chunkCuncorrency);
            var fileHashingResult = await fileHasherPipeline.HashDataAsync(stream).ConfigureAwait(false);
            
            // If file name is null or empty, use the file hash as name.
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = fileHashingResult.Hash.ToString();
            
            // Create manifest.
            var manifest = new MantarayManifest(
                readOnlyPipeline => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    redundancyLevel,
                    encrypt,
                    0,
                    chunkCuncorrency,
                    readOnlyPipeline),
                compactLevel);

            manifest.Add(
                MantarayManifest.RootPath,
                ManifestEntry.NewDirectory(
                    new Dictionary<string, string>
                    {
                        [ManifestEntry.WebsiteIndexDocPathKey] = fileName,
                    }));

            var fileEntryMetadata = new Dictionary<string, string>
            {
                [ManifestEntry.ContentTypeKey] = fileContentType,
                [ManifestEntry.FilenameKey] = fileName
            };
            if (fileHashingResult.EncryptionKey != null)
                fileEntryMetadata.Add(ManifestEntry.ChunkEncryptKeyKey, fileHashingResult.EncryptionKey.Value.ToString());
            if (fileHashingResult.UseRecursiveEncryption)
                fileEntryMetadata.Add(ManifestEntry.UseRecursiveEncryptionKey, true.ToString());
            
            manifest.Add(
                fileName,
                ManifestEntry.NewFile(
                    fileHashingResult.Hash,
                    fileEntryMetadata));

            var chunkHashingResult = await manifest.GetHashAsync(hasher).ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                chunkHashingResult,
                fileHasherPipeline.MissedOptimisticHashing,
                postageStamper.StampIssuer);
        }

        public Task<SwarmHash> WriteDataChunksAsync(
            IChunkStore chunkStore,
            byte[] data,
            IPostageStampIssuer? postageStampIssuer = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            int? chunkCuncorrency = null)
        {
            using var stream = new MemoryStream(data);
            return WriteDataChunksAsync(
                chunkStore,
                stream,
                postageStampIssuer,
                compactLevel,
                encrypt,
                redundancyLevel,
                chunkCuncorrency);
        }

        public async Task<SwarmHash> WriteDataChunksAsync(
            IChunkStore chunkStore,
            Stream stream,
            IPostageStampIssuer? postageStampIssuer = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            int? chunkCuncorrency = null)
        {
            postageStampIssuer ??= new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            var postageStamper = new PostageStamper(
                new FakeSigner(),
                postageStampIssuer,
                new MemoryStampStore());
            
            // Create chunks and get file hash.
            using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                chunkStore,
                postageStamper,
                redundancyLevel,
                encrypt,
                compactLevel,
                chunkCuncorrency);
            var fileHashingResult = await fileHasherPipeline.HashDataAsync(stream).ConfigureAwait(false);
            
            // Return file hash.
            return fileHashingResult.Hash;
        }
    }
}