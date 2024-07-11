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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Store
{
    public class ChunkJoiner(
        IReadOnlyChunkStore chunkStore)
    {
        // Methods.
        public async Task<IEnumerable<byte>> GetJoinedChunkDataAsync(SwarmHash hash)
        {
            var chunk = await chunkStore.GetAsync(hash).ConfigureAwait(false);
            var totalDataLength = SwarmChunk.SpanToLength(chunk.Span.Span);
            
            if (totalDataLength <= SwarmChunk.DataSize)
                return chunk.Data.ToArray();
            
            var joinedData = new List<byte>();
                
            for (int i = 0; i < chunk.Data.Length; i += SwarmHash.HashSize)
            {
                var childHash = new SwarmHash(chunk.Data[i..(i + SwarmHash.HashSize)].ToArray());
                joinedData.AddRange(await GetJoinedChunkDataAsync(childHash).ConfigureAwait(false));
            }
            
            return joinedData;
        }
    }
}