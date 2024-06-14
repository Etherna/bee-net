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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Store
{
    /// <summary>
    /// Store chunks in a local directory
    /// </summary>
    [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public class LocalDirectoryChunkStore : IChunkStore
    {
        // Constructor.
        public LocalDirectoryChunkStore(string directoryPath, bool createDirectory = false)
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
            var files = Directory.GetFiles(DirectoryPath, "*.chunk");
            var hashes = new List<SwarmHash>();
            
            foreach (var file in files)
            {
                try { hashes.Add(new SwarmHash(Path.GetFileName(file))); }
                catch { }
            }

            return Task.FromResult<IEnumerable<SwarmHash>>(hashes);
        }

        public async Task<SwarmChunk?> TryGetAsync(SwarmHash hash)
        {
            var chunkPath = Path.Combine(DirectoryPath, hash.ToString());
            
            if (!File.Exists(chunkPath))
                return null;

            var buffer = new byte[SwarmChunk.SpanAndDataSize];
            using var fileStream = File.OpenRead(chunkPath);
            var readBytes = await fileStream.ReadAsync(buffer).ConfigureAwait(false);

            var chunk = SwarmChunk.BuildFromSpanAndData(hash, buffer.AsSpan()[..readBytes]);
            return chunk;
        }

        public async Task<bool> AddAsync(SwarmChunk chunk)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));

            var chunkPath = Path.Combine(DirectoryPath, chunk.Hash + ".chunk");
            
            if (File.Exists(chunkPath))
                return false;
            
            var tmpChunkPath = Path.GetTempFileName();

            try
            {
                //write tmp file, and complete writing.
                using (var fileStream = File.Create(tmpChunkPath))
                {
                    await fileStream.WriteAsync(chunk.GetSpanAndData()).ConfigureAwait(false);
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