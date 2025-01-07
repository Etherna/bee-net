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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    public class ChunkJoiner(
        IReadOnlyChunkStore chunkStore)
    {
        // Methods.
        /// <summary>
        /// Get data stream from chunks
        /// </summary>
        /// <param name="rootChunkReference">The root chunk reference</param>
        /// <param name="fileCachePath">Optional file where store read data. Necessary if data is >2GB</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The data stream</returns>
        public async Task<Stream> GetJoinedChunkDataAsync(
            SwarmChunkReference rootChunkReference,
            string? fileCachePath = null,
            CancellationToken? cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(rootChunkReference, nameof(rootChunkReference));

            //in memory
            if (fileCachePath is null)
            {
                var dataStream = new MemoryStream();
                
                await GetJoinedChunkDataHelperAsync(
                    rootChunkReference,
                    dataStream,
                    cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
                
                dataStream.Position = 0;
                return dataStream;
            }

            //file cached
            using (var writeDataStream = File.OpenWrite(fileCachePath))
            {
                await GetJoinedChunkDataHelperAsync(
                    rootChunkReference,
                    writeDataStream,
                    cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
                    
                await writeDataStream.FlushAsync().ConfigureAwait(false);
            }

            return File.OpenRead(fileCachePath);
        }

        // Helpers.
        private async Task GetJoinedChunkDataHelperAsync(
            SwarmChunkReference chunkReference,
            Stream dataStream,
            CancellationToken cancellationToken)
        {
            // Read and decrypt chunk data.
            var chunk = await chunkStore.GetAsync(chunkReference.Hash).ConfigureAwait(false);
            var dataArray = chunk.Data.ToArray();
            chunkReference.EncryptionKey?.EncryptDecrypt(dataArray);
            
            // Determine if is a data chunk, or an intermediate chunk.
            var totalDataLength = SwarmChunk.SpanToLength(chunk.Span.Span);
            
            //if is data chunk
            if (totalDataLength <= SwarmChunk.DataSize)
            {
                await dataStream.WriteAsync(dataArray, cancellationToken).ConfigureAwait(false);
                return;
            }
            
            //else, is intermediate chunk
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
                await GetJoinedChunkDataHelperAsync(
                    new SwarmChunkReference(
                        childHash,
                        childEncryptionKey,
                        chunkReference.UseRecursiveEncryption),
                    dataStream,
                    cancellationToken).ConfigureAwait(false);
            }
        }
    }
}