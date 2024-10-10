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

using Etherna.BeeNet.Models;
using System.Collections.Generic;

namespace Etherna.BeeNet.Manifest
{
    public class ManifestEntry
    {
        // Consts.
        public const string ChunkEncryptKeyKey = "ChunkEncryptKey";
        public const string ContentTypeKey = "Content-Type";
        public const string FilenameKey = "Filename";
        public const string WebsiteErrorDocPathKey = "website-error-document";
        public const string WebsiteIndexDocPathKey = "website-index-document";
        public const string UseRecursiveEncryptionKey = "recursiveEncrypt";
        
        // Constructor.
        private ManifestEntry(
            SwarmHashTree hashTree,
            IReadOnlyDictionary<string, string> metadata)
        {
            HashTree = hashTree;
            Metadata = metadata;
        }
        
        // Static builders.
        public static ManifestEntry NewDirectory(
            IReadOnlyDictionary<string, string> metadata) =>
            new(new SwarmHashTree(
                    new SwarmChunkReference(SwarmHash.Zero, null, false),
                    []),
                metadata);

        public static ManifestEntry NewFile(
            SwarmHashTree fileHashTree,
            IReadOnlyDictionary<string, string> metadata) =>
            new(fileHashTree, metadata);
        
        // Properties.
        public SwarmHashTree HashTree { get; }
        public bool IsDirectory => HashTree.ChunkRef.Hash == SwarmHash.Zero;
        public IReadOnlyDictionary<string, string> Metadata { get; }
    }
}