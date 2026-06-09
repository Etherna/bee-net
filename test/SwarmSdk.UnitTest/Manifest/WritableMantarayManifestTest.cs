// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
//
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.SwarmSdk.Hashing;
using Etherna.SwarmSdk.Hashing.Pipeline;
using Etherna.SwarmSdk.Hashing.Postage;
using Etherna.SwarmSdk.Models;
using Etherna.SwarmSdk.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.SwarmSdk.Manifest
{
    public class WritableMantarayManifestTest
    {
        // Internal classes.
        private sealed record OriginalManifestData(
            IChunkStore ChunkStore,
            IPostageStamper PostageStamper,
            SwarmReference RootReference,
            IReadOnlyDictionary<string, ManifestEntry> Entries);

        // Data.
        private static readonly (string Path, string ContentType, string Filename, int SizeKb)[] TestFiles =
        [
            ("assets/app.js", "application/javascript", "app.js", 8),
            ("assets/style.css", "text/css", "style.css", 2),
            ("docs/readme.md", "text/markdown", "readme.md", 6),
            ("index.html", "text/html", "index.html", 4)
        ];

        // Tests.
        [Fact]
        public async Task BuildFromReferencedManifestAsync_PreservesAllEntriesAndMetadata()
        {
            // Setup.
            var original = await BuildOriginalManifestAsync();
            var referencedManifest = ReferencedMantarayManifest.BuildNew(
                original.RootReference,
                original.ChunkStore);

            // Run.
            var writableManifest = await WritableMantarayManifest.BuildFromReferencedManifestAsync(
                referencedManifest,
                original.ChunkStore,
                original.PostageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);

            // Assert.
            foreach (var (path, entry) in original.Entries)
            {
                var resourceInfo = await writableManifest.RootNode.GetResourceInfoAsync(path);

                Assert.Equal(entry.Reference, resourceInfo.Reference);
                Assert.Equal(
                    entry.Metadata[ManifestEntry.ContentTypeKey],
                    resourceInfo.Metadata[ManifestEntry.ContentTypeKey]);
                Assert.Equal(
                    entry.Metadata[ManifestEntry.FilenameKey],
                    resourceInfo.Metadata[ManifestEntry.FilenameKey]);
            }
        }

        [Fact]
        public async Task BuildFromReferencedManifestAsync_ReproducesOriginalReference()
        {
            // Setup.
            var original = await BuildOriginalManifestAsync();
            var referencedManifest = ReferencedMantarayManifest.BuildNew(
                original.RootReference,
                original.ChunkStore);

            // Run.
            var writableManifest = await WritableMantarayManifest.BuildFromReferencedManifestAsync(
                referencedManifest,
                original.ChunkStore,
                original.PostageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);
            var rebuiltReference = await writableManifest.GetReferenceAsync(new Hasher());

            // Assert.
            // The converted manifest rebuilds the exact same structure, so it hashes to the same reference.
            Assert.Equal(original.RootReference, rebuiltReference);
        }

        [Fact]
        public async Task BuildFromReferencedManifestAsync_ResultIsEditable()
        {
            // Setup.
            var original = await BuildOriginalManifestAsync();
            var referencedManifest = ReferencedMantarayManifest.BuildNew(
                original.RootReference,
                original.ChunkStore);
            var writableManifest = await WritableMantarayManifest.BuildFromReferencedManifestAsync(
                referencedManifest,
                original.ChunkStore,
                original.PostageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);

            // Run.
            var newFileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                original.ChunkStore,
                original.PostageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);
            var newFileData = new byte[1024];
            new Random(1).NextBytes(newFileData);
            var newFileReference = await newFileHasherPipeline.HashDataAsync(newFileData);

            writableManifest.Add(
                "newfile.txt",
                ManifestEntry.NewFile(
                    newFileReference,
                    new Dictionary<string, string> { [ManifestEntry.FilenameKey] = "newfile.txt" }));

            // Assert.
            // The added entry is reachable in the editable tree.
            var newResourceInfo = await writableManifest.RootNode.GetResourceInfoAsync("newfile.txt");
            Assert.Equal(newFileReference, newResourceInfo.Reference);

            // Hashing the edited manifest produces a reference different from the original one.
            var editedReference = await writableManifest.GetReferenceAsync(new Hasher());
            Assert.NotEqual(original.RootReference, editedReference);
        }

        [Fact]
        public async Task BuildFromReferencedManifestAsync_ThrowsOnNullArguments()
        {
            // Setup.
            var original = await BuildOriginalManifestAsync();
            var referencedManifest = ReferencedMantarayManifest.BuildNew(
                original.RootReference,
                original.ChunkStore);

            // Run & Assert.
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                WritableMantarayManifest.BuildFromReferencedManifestAsync(
                    null!,
                    original.ChunkStore,
                    original.PostageStamper,
                    RedundancyLevel.None,
                    false,
                    0,
                    null));
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                WritableMantarayManifest.BuildFromReferencedManifestAsync(
                    referencedManifest,
                    null!,
                    original.PostageStamper,
                    RedundancyLevel.None,
                    false,
                    0,
                    null));
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                WritableMantarayManifest.BuildFromReferencedManifestAsync(
                    referencedManifest,
                    original.ChunkStore,
                    null!,
                    RedundancyLevel.None,
                    false,
                    0,
                    null));
        }

        // Helpers.
        private static async Task<OriginalManifestData> BuildOriginalManifestAsync()
        {
            var chunkStore = new MemoryChunkStore();
            var postageStamper = new FakePostageStamper();
            var manifest = new WritableMantarayManifest(
                chunkStore,
                postageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);

            var entries = new Dictionary<string, ManifestEntry>();
            var fileDataRand = new Random(0);

            foreach (var (path, contentType, filename, sizeKb) in TestFiles)
            {
                var fileData = new byte[sizeKb * 1024];
                fileDataRand.NextBytes(fileData);

                var hasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    RedundancyLevel.None,
                    false,
                    0,
                    null);
                var fileReference = await hasherPipeline.HashDataAsync(fileData);

                var entry = ManifestEntry.NewFile(
                    fileReference,
                    new Dictionary<string, string>
                    {
                        [ManifestEntry.ContentTypeKey] = contentType,
                        [ManifestEntry.FilenameKey] = filename
                    });

                manifest.Add(path, entry);
                entries[path] = entry;
            }

            var rootReference = await manifest.GetReferenceAsync(new Hasher());

            return new OriginalManifestData(chunkStore, postageStamper, rootReference, entries);
        }
    }
}
