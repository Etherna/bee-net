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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Models;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Store
{
    public class BeeClientChunkStore(IBeeClient beeClient)
        : IReadOnlyChunkStore
    {
        public Task<SwarmChunk> GetAsync(SwarmHash hash) =>
            beeClient.GetChunkAsync(hash);

        public async Task<SwarmChunk?> TryGetAsync(SwarmHash hash)
        {
            try
            {
                return await beeClient.GetChunkAsync(hash).ConfigureAwait(false);
            }
            catch (BeeNetApiException)
            {
                return null;
            }
        }
    }
}