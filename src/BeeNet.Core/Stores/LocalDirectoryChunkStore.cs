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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Stores
{
    /// <summary>
    /// Store chunks in a local directory
    /// </summary>
    [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public class LocalDirectoryChunkStore : ChunkStoreBase
    {
        // Consts.
        public const string ChunkFileExtension = ".chunk";
        
        // Constructor.
        public LocalDirectoryChunkStore(
            string directoryPath,
            bool createDirectory = false,
            ConcurrentDictionary<SwarmHash, SwarmChunk>? chunksCache = null)
            : base(chunksCache)
        {
            if (!Directory.Exists(directoryPath))
            {
                if (createDirectory)
                    Directory.CreateDirectory(directoryPath);
                else
                    throw new IOException($"Directory \"{directoryPath}\" doesn't exist");
            }

            DirectoryPath = directoryPath;
        }

        // Properties.
        public string DirectoryPath { get; }

        // Methods.
        public Task<IEnumerable<SwarmHash>> GetAllHashesAsync()
        {
            var files = Directory.GetFiles(DirectoryPath, '*' + ChunkFileExtension);
            var hashes = new List<SwarmHash>();
            
            foreach (var file in files)
            {
                try { hashes.Add(new SwarmHash(Path.GetFileName(file))); }
                catch { }
            }

            return Task.FromResult<IEnumerable<SwarmHash>>(hashes);
        }

        // Protected methods.
        protected override Task<bool> DeleteChunkAsync(SwarmHash hash)
        {
            var chunkPath = Path.Combine(DirectoryPath, hash + ChunkFileExtension);

            if (!File.Exists(chunkPath))
                return Task.FromResult(false);
            
            File.Delete(chunkPath);
            return Task.FromResult(true);
        }

        protected override async Task<SwarmChunk> LoadChunkAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default)
        {
            var chunkPath = Path.Combine(DirectoryPath, hash + ChunkFileExtension);
            
            if (!File.Exists(chunkPath))
                throw new KeyNotFoundException($"Chunk {hash} doesnt' exist");

            var buffer = new byte[SwarmChunk.SpanDataSize];
            var fileStream = File.OpenRead(chunkPath);
            await using var stream = fileStream.ConfigureAwait(false);
            var readBytes = await fileStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            return new SwarmChunk(hash, buffer.AsMemory()[..readBytes]);
        }

        protected override async Task<bool> SaveChunkAsync(SwarmChunk chunk)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));

            var chunkPath = Path.Combine(DirectoryPath, chunk.Hash + ChunkFileExtension);
            
            if (File.Exists(chunkPath))
                return false;
            
            var tmpChunkPath = Path.GetTempFileName();

            try
            {
                //write tmp file, and complete writing.
                using (var fileStream = File.Create(tmpChunkPath))
                {
                    await fileStream.WriteAsync(chunk.SpanData).ConfigureAwait(false);
                    await fileStream.FlushAsync().ConfigureAwait(false);
                }
                
                //rename it. This method prevent concurrent reading/writing 
                File.Move(tmpChunkPath, chunkPath, overwrite: true);
            }
            catch
            {
                //remove tmp chunk file in case of error
                File.Delete(tmpChunkPath);
            }
            
            return true;
        }
    }
}