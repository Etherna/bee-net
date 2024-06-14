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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Store
{
    public interface IChunkStore
    {
        public Task<IEnumerable<SwarmHash>> GetAllHashesAsync();
        
        public Task<SwarmChunk> GetAsync(SwarmHash hash);
        
        public Task<SwarmChunk?> TryGetAsync(SwarmHash hash);

        /// <summary>
        /// Add a chunk in the store
        /// </summary>
        /// <param name="chunk">The chuck to add</param>
        /// <returns>True if chunk has been added, false if already existing</returns>
        public Task<bool> AddAsync(SwarmChunk chunk);
    }
}