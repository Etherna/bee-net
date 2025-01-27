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
using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Hashing.Signer;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public class ChunkService : IChunkService
    {
        public async Task<UploadEvaluationResult> EvaluateDirectoryUploadAsync(
            string directoryPath,
            string? indexFilename = null,
            string? errorFilename = null,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            int? chunkCuncorrency = null, 
            IChunkStore? chunkStore = null)
        {
            // Checks.
            if (indexFilename?.Contains(SwarmAddress.Separator, StringComparison.InvariantCulture) == true)
                throw new ArgumentException(
                    "Index document suffix must not include slash character",
                    nameof(indexFilename));
            
            // Init.
            chunkStore ??= new FakeChunkStore();
            
            postageStampIssuer ??= new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            var postageStamper = new PostageStamper(
                new FakeSigner(),
                postageStampIssuer,
                new MemoryStampStore());

            long totalMissedOptimisticHashing = 0;
            
            // Try set index document.
            if (indexFilename is null &&
                File.Exists(Path.Combine(directoryPath, "index.html")))
                indexFilename = "index.html";
            
            // Create manifest.
            var dirManifest = new MantarayManifest(
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    redundancyLevel,
                    encrypt,
                    0,
                    chunkCuncorrency),
                encrypt);

            // Iterate through the files in the supplied directory.
            var files = Directory.GetFiles(directoryPath, "", SearchOption.AllDirectories);
            if (files.Length == 0)
                throw new ArgumentException("No files in root directory", nameof(directoryPath));
            
            foreach (var file in files)
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
                using var fileStream = File.OpenRead(file);

                var fileHashingResult = await fileHasherPipeline.HashDataAsync(fileStream).ConfigureAwait(false);
                totalMissedOptimisticHashing += fileHasherPipeline.MissedOptimisticHashing;
                
                // Add file entry to dir manifest.
                var fileEntryMetadata = new Dictionary<string, string>
                {
                    [ManifestEntry.ContentTypeKey] = fileContentType,
                    [ManifestEntry.FilenameKey] = fileName
                };
                if (fileHashingResult.EncryptionKey != null)
                    fileEntryMetadata.Add(ManifestEntry.ChunkEncryptKeyKey, fileHashingResult.EncryptionKey.ToString());
                if (compactLevel > 0)
                    fileEntryMetadata.Add(ManifestEntry.UseRecursiveEncryptionKey, true.ToString());
                
                dirManifest.Add(
                    Path.GetRelativePath(directoryPath, file),
                    ManifestEntry.NewFile(
                        fileHashingResult.Hash,
                        fileEntryMetadata));
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
            var chunkHashingResult = await dirManifest.GetHashAsync().ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                chunkHashingResult,
                totalMissedOptimisticHashing,
                postageStampIssuer);
        }

        public async Task<UploadEvaluationResult> EvaluateSingleFileUploadAsync(
            byte[] data,
            string fileContentType,
            string? fileName,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            int? chunkCuncorrency = null, 
            IChunkStore? chunkStore = null)
        {
            using var stream = new MemoryStream(data);
            return await EvaluateSingleFileUploadAsync(
                stream,
                fileContentType,
                fileName,
                compactLevel,
                encrypt,
                redundancyLevel,
                postageStampIssuer,
                chunkCuncorrency,
                chunkStore).ConfigureAwait(false);
        }

        public async Task<UploadEvaluationResult> EvaluateSingleFileUploadAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            int? chunkCuncorrency = null, 
            IChunkStore? chunkStore = null)
        {
            // Validate input.
            if (fileName?.Contains('/', StringComparison.InvariantCulture) == true)
                throw new ArgumentException("Invalid name", nameof(fileName));

            // Init.
            chunkStore ??= new FakeChunkStore();
            postageStampIssuer ??= new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            var postageStamper = new PostageStamper(
                new FakeSigner(),
                postageStampIssuer,
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
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    redundancyLevel,
                    encrypt,
                    0,
                    chunkCuncorrency),
                encrypt);

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
                fileEntryMetadata.Add(ManifestEntry.ChunkEncryptKeyKey, fileHashingResult.EncryptionKey.ToString());
            if (compactLevel > 0)
                fileEntryMetadata.Add(ManifestEntry.UseRecursiveEncryptionKey, true.ToString());
            
            manifest.Add(
                fileName,
                ManifestEntry.NewFile(
                    fileHashingResult.Hash,
                    fileEntryMetadata));

            var chunkHashingResult = await manifest.GetHashAsync().ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                chunkHashingResult,
                fileHasherPipeline.MissedOptimisticHashing,
                postageStampIssuer);
        }

        public string[] GetAllChunkFilesInDirectory(string chunkStoreDirectory) =>
            Directory.GetFiles(chunkStoreDirectory, '*' + LocalDirectoryChunkStore.ChunkFileExtension);

        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataFromChunksAsync(
            SwarmAddress address,
            IReadOnlyChunkStore chunkStore)
        {
            var rootManifest = new ReferencedMantarayManifest(
                chunkStore,
                address.Hash);
            
            return await rootManifest.GetResourceMetadataAsync(address).ConfigureAwait(false);
        }

        public async Task<Stream> GetFileStreamFromChunksAsync(
            string chunkStoreDirectory,
            SwarmAddress address,
            string? fileCachePath = null,
            CancellationToken? cancellationToken = default)
        {
            var chunkStore = new LocalDirectoryChunkStore(chunkStoreDirectory);
            var chunkJoiner = new ChunkJoiner(chunkStore);
            
            var rootManifest = new ReferencedMantarayManifest(
                chunkStore,
                address.Hash);
            
            var chunkReference = await rootManifest.ResolveAddressToChunkReferenceAsync(address.Path).ConfigureAwait(false);
            
            return await chunkJoiner.GetJoinedChunkDataAsync(
                chunkReference,
                fileCachePath,
                cancellationToken).ConfigureAwait(false);
        }

        public Task<SwarmHash> WriteDataChunksAsync(
            byte[] data,
            string outputDirectory,
            IPostageStampIssuer? postageStampIssuer = null,
            bool createDirectory = true,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            int? chunkCuncorrency = null)
        {
            using var stream = new MemoryStream(data);
            return WriteDataChunksAsync(
                stream,
                outputDirectory,
                postageStampIssuer,
                createDirectory,
                compactLevel,
                encrypt,
                redundancyLevel,
                chunkCuncorrency);
        }

        public async Task<SwarmHash> WriteDataChunksAsync(
            Stream stream,
            string outputDirectory,
            IPostageStampIssuer? postageStampIssuer = null,
            bool createDirectory = true,
            ushort compactLevel = 0,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            int? chunkCuncorrency = null)
        {
            var chunkStore = new LocalDirectoryChunkStore(outputDirectory, createDirectory);
            
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