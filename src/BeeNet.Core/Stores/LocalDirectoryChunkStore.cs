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
using System.Linq;
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
        public const string CacFileExtension = ".cac";
        public const string SocFileExtension = ".soc";
        
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
            var cacFiles = Directory.GetFiles(DirectoryPath, '*' + CacFileExtension);
            var socFiles = Directory.GetFiles(DirectoryPath, '*' + SocFileExtension);
            var hashes = new HashSet<SwarmHash>();
            
            foreach (var file in cacFiles.Concat(socFiles))
            {
                try { hashes.Add(new SwarmHash(Path.GetFileName(file))); }
                catch { }
            }

            return Task.FromResult<IEnumerable<SwarmHash>>(hashes);
        }

        // Protected methods.
        protected override Task<bool> DeleteChunkAsync(SwarmHash hash)
        {
            // Try cac.
            var cacPath = Path.Combine(DirectoryPath, hash + CacFileExtension);
            if (File.Exists(cacPath))
            {
                File.Delete(cacPath);
                return Task.FromResult(true);
            }
            
            // Try soc.
            var socPath = Path.Combine(DirectoryPath, hash + SocFileExtension);
            if (File.Exists(socPath))
            {
                File.Delete(socPath);
                return Task.FromResult(true);
            }
            
            return Task.FromResult(false);
        }

        protected override async Task<SwarmChunk> LoadChunkAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default)
        {
            // Try cac.
            var cacPath = Path.Combine(DirectoryPath, hash + CacFileExtension);
            if (File.Exists(cacPath))
            {
                var buffer = new byte[SwarmCac.SpanDataSize];
                var fileStream = File.OpenRead(cacPath);
                await using var stream = fileStream.ConfigureAwait(false);
                var readBytes = await fileStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

                return new SwarmCac(hash, buffer.AsMemory()[..readBytes]);
            }
            
            // Try soc.
            var socPath = Path.Combine(DirectoryPath, hash + SocFileExtension);
            if (File.Exists(socPath))
            {
                var buffer = new byte[SwarmSoc.MaxSocSize];
                var fileStream = File.OpenRead(socPath);
                await using var stream = fileStream.ConfigureAwait(false);
                var readBytes = await fileStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

                return SwarmSoc.BuildFromBytes(hash, buffer.AsMemory()[..readBytes], new SwarmChunkBmt());
            }

            throw new KeyNotFoundException($"Chunk {hash} doesnt' exist");
        }

        protected override async Task<bool> SaveChunkAsync(SwarmChunk chunk)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));

            var chunkFileExtension = chunk switch
            {
                SwarmCac => CacFileExtension,
                SwarmSoc => SocFileExtension,
                _ => throw new InvalidOperationException(),
            };
            var chunkPath = Path.Combine(DirectoryPath, chunk.Hash + chunkFileExtension);
            
            if (File.Exists(chunkPath))
                return false;
            
            var tmpChunkPath = Path.GetTempFileName();

            try
            {
                //write tmp file, and complete writing.
                using (var fileStream = File.Create(tmpChunkPath))
                {
                    await fileStream.WriteAsync(chunk.GetFullPayload()).ConfigureAwait(false);
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