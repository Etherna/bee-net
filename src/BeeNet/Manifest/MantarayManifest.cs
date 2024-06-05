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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class MantarayManifest
    {
        // Consts.
        public const string RootPath = "/";
        
        // Fields.
        private readonly Func<IHasherPipeline> hasherBuilder;
        private readonly MantarayNode trie;

        // Constructor.
        public MantarayManifest(
            Func<IHasherPipeline> hasherBuilder,
            bool isEncrypted)
        {
            this.hasherBuilder = hasherBuilder;
            trie = new MantarayNode(isEncrypted);
        }

        // Methods.
        public void Add(string path, ManifestEntry entry)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(entry, nameof(entry));

            var p = Encoding.UTF8.GetBytes(path);
            var e = entry.Address.ToByteArray();
            trie.Add(p, e, entry.Metadata);
        }

        public async Task<SwarmAddress> GetHashAsync()
        {
            await trie.SaveAsync(hasherBuilder).ConfigureAwait(false);
            return new SwarmAddress(trie.Reference!);
        }
    }
}