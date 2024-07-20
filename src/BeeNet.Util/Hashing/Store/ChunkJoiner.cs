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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Store
{
    public class ChunkJoiner(
        IReadOnlyChunkStore chunkStore)
    {
        // Methods.
        public async Task<IEnumerable<byte>> GetJoinedChunkDataAsync(SwarmChunkReference chunkReference)
        {
            ArgumentNullException.ThrowIfNull(chunkReference, nameof(chunkReference));
            
            var chunk = await chunkStore.GetAsync(chunkReference.Hash).ConfigureAwait(false);
            
            var dataArray = chunk.Data.ToArray();
            chunkReference.EncryptionKey?.EncryptDecrypt(dataArray);
            
            var totalDataLength = SwarmChunk.SpanToLength(chunk.Span.Span);
            if (totalDataLength <= SwarmChunk.DataSize)
                return dataArray;
            
            var joinedData = new List<byte>();
            for (int i = 0; i < dataArray.Length;)
            {
                //read hash
                var childHash = new SwarmHash(dataArray[i..(i + SwarmHash.HashSize)]);
                i += SwarmHash.HashSize;
                
                //read encryption key
                XorEncryptKey? childEncryptionKey = null;
                if (chunkReference.UseRecursiveEncryption)
                {
                    childEncryptionKey = new XorEncryptKey(dataArray[i..(i + XorEncryptKey.KeySize)]);
                    i += XorEncryptKey.KeySize;
                }
                
                //add joined data recursively
                joinedData.AddRange(await GetJoinedChunkDataAsync(
                    new SwarmChunkReference(
                        childHash,
                        childEncryptionKey,
                        chunkReference.UseRecursiveEncryption)).ConfigureAwait(false));
            }
            
            return joinedData;
        }
    }
}