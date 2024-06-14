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

using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public class ReferencedMantarayManifest : IReadOnlyMantarayManifest
    {
        // Constructors.
        public ReferencedMantarayManifest(
            IChunkStore chunkStore,
            SwarmHash rootHash)
        {
            RootNode = new ReferencedMantarayNode(chunkStore, rootHash);
        }
        
        // Properties.
        public IReadOnlyMantarayNode RootNode { get; }

        // Methods.
        public Task<SwarmHash> GetHashAsync() => Task.FromResult(RootNode.Hash);

        public Task<SwarmHash> ResolveResourceHashAsync(SwarmAddress address)
        {
            throw new NotImplementedException();
        }
    }
}