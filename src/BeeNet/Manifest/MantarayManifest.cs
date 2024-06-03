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

using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Models;
using System;
using System.Text;

namespace Etherna.BeeNet.Manifest
{
    public class MantarayManifest
    {
        // Consts.
        public const string EntryMetadataContentTypeKey = "Content-Type";
        public const string EntryMetadataFilenameKey = "Filename";
        public const string RootPath = "/";
        public const string WebsiteErrorDocumentPathKey = "website-error-document";
        public const string WebsiteIndexDocumentSuffixKey = "website-index-document";
        
        // Fields.
        private readonly IHasherPipeline hasherPipeline;
        private readonly MantarayNode trie;

        // Constructor.
        public MantarayManifest(
            IHasherPipeline hasherPipeline,
            bool isEncrypted)
        {
            this.hasherPipeline = hasherPipeline;
            trie = new MantarayNode();

            // Use empty obfuscation key if not encrypting.
            if (!isEncrypted)
            {
                // NOTE: it will be copied to all trie nodes
                trie.ObfuscationKey = MantarayNode.ZeroObfuscationKey;
            }
        }

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));

            var p = Encoding.UTF8.GetBytes(path);
            var e = entry.Reference.ToByteArray();
            trie.Add(p, e, entry.Metadata, hasherPipeline);
        }

        public SwarmAddress Store()
        {
            var ls mantaray.LoadSaver
            if len(storeSizeFn) > 0 {
                ls = &mantarayLoadSaver{
                    ls:          m.ls,
                    storeSizeFn: storeSizeFn,
                }
            } else {
                ls = m.ls
            }

            err := m.trie.Save(ctx, ls)
            if err != nil {
                return swarm.ZeroAddress, fmt.Errorf("manifest save error: %w", err)
            }

            address := swarm.NewAddress(m.trie.Reference())

            return address, nil
        }
    }
}