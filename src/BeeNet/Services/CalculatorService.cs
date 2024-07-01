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

using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Hasher.Postage;
using Etherna.BeeNet.Hasher.Signer;
using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public class CalculatorService : ICalculatorService
    {
        public async Task<UploadEvaluationResult> EvaluateDirectoryUploadAsync(
            string directoryPath,
            string? indexFilename = null,
            string? errorFilename = null,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null)
        {
            chunkStore ??= new FakeChunkStore();
            
            postageStampIssuer ??= new PostageStampIssuer(PostageBatch.MaxDepthInstance);
            var postageStamper = new PostageStamper(
                new FakeSigner(),
                postageStampIssuer,
                new MemoryStampStore());
            
            // Create manifest.
            var dirManifest = new MantarayManifest(
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    redundancyLevel,
                    encrypt),
                encrypt);
            
            if (indexFilename?.Contains('/', StringComparison.InvariantCulture) == true)
                throw new ArgumentException(
                    "Index document suffix must not include slash character",
                    nameof(indexFilename));

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
                    encrypt);
                
                var fileContentType = FileContentTypeProvider.GetContentType(file);
                var fileName = Path.GetFileName(file);
                using var fileStream = File.OpenRead(file);

                var fileHash = await fileHasherPipeline.HashDataAsync(fileStream).ConfigureAwait(false);
                
                // Add file entry to dir manifest.
                dirManifest.Add(
                    Path.GetRelativePath(directoryPath, file),
                    ManifestEntry.NewFile(fileHash, new Dictionary<string, string>
                    {
                        [ManifestEntry.ContentTypeKey] = fileContentType,
                        [ManifestEntry.FilenameKey] = fileName
                    }));
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
            var manifestHash = await dirManifest.GetHashAsync().ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                manifestHash,
                postageStampIssuer);
        }

        public async Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            byte[] data,
            string fileContentType,
            string? fileName,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null)
        {
            using var stream = new MemoryStream(data);
            return await EvaluateFileUploadAsync(
                stream,
                fileContentType,
                fileName,
                encrypt,
                redundancyLevel,
                postageStampIssuer,
                chunkStore).ConfigureAwait(false);
        }

        public async Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            Stream stream,
            string fileContentType,
            string? fileName,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null,
            IChunkStore? chunkStore = null)
        {
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
                encrypt);
            var fileHash = await fileHasherPipeline.HashDataAsync(stream).ConfigureAwait(false);
            fileName ??= fileHash.ToString(); //if missing, set file name with its address
            
            // Create manifest.
            var manifest = new MantarayManifest(
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    redundancyLevel,
                    encrypt),
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

            var manifestHash = await manifest.GetHashAsync().ConfigureAwait(false);
            
            // Return result.
            return new UploadEvaluationResult(
                manifestHash,
                postageStampIssuer);
        }

        public async Task<IReadOnlyDictionary<string, string>> GetResourceMetadataFromChunksAsync(
            string chunkStoreDirectory,
            SwarmAddress address)
        {
            var chunkStore = new LocalDirectoryChunkStore(chunkStoreDirectory);
            
            var rootManifest = new ReferencedMantarayManifest(
                chunkStore,
                address.Hash);
            
            return await rootManifest.GetResourceMetadataAsync(address).ConfigureAwait(false);
        }

        public async Task<Stream> GetResourceStreamFromChunksAsync(
            string chunkStoreDirectory,
            SwarmAddress address)
        {
            var chunkStore = new LocalDirectoryChunkStore(chunkStoreDirectory);
            var chunkJoiner = new ChunkJoiner(chunkStore);
            
            var rootManifest = new ReferencedMantarayManifest(
                chunkStore,
                address.Hash);
            
            var resourceHash = await rootManifest.ResolveResourceHashAsync(address).ConfigureAwait(false);
            
            var memoryStream = new MemoryStream();
            var resourceData = await chunkJoiner.GetJoinedChunkDataAsync(resourceHash).ConfigureAwait(false);
            memoryStream.Write(resourceData.ToArray());
            memoryStream.Position = 0;
            
            return memoryStream;
        }
    }
}