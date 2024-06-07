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
using System.Collections.Generic;

namespace Etherna.BeeNet.Manifest
{
    public class ManifestEntry
    {
        // Consts.
        public const string ContentTypeKey = "Content-Type";
        public const string FilenameKey = "Filename";
        public const string WebsiteErrorDocPathKey = "website-error-document";
        public const string WebsiteIndexDocPathKey = "website-index-document";
        
        // Constructor.
        private ManifestEntry(
            SwarmAddress address,
            IReadOnlyDictionary<string, string> metadata)
        {
            Address = address;
            Metadata = metadata;
        }
        
        // Static builders.
        public static ManifestEntry NewDirectory(
            IReadOnlyDictionary<string, string> metadata) =>
            new(SwarmAddress.Zero, metadata);

        public static ManifestEntry NewFile(
            SwarmAddress fileAddress,
            IReadOnlyDictionary<string, string> metadata) =>
            new(fileAddress, metadata);
        
        // Properties.
        public SwarmAddress Address { get; }
        public bool IsDirectory => Address == SwarmAddress.Zero;
        public IReadOnlyDictionary<string, string> Metadata { get; }
    }
}