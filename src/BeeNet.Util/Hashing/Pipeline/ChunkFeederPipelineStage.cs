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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    /// <summary>
    /// Produce chunked data with span prefix for successive stages.
    /// Also controls the parallelism on chunk elaboration
    /// </summary>
    internal sealed class ChunkFeederPipelineStage(
        IHasherPipelineStage nextStage,
        int chunkConcurrency)
        : IHasherPipeline
    {
        // Fields.
        private readonly List<Task> nextStageTasks = new();
        private readonly SemaphoreSlim semaphore = new(chunkConcurrency, chunkConcurrency);
        
        private long passedBytes;
        
        // Constructor.
        public ChunkFeederPipelineStage(IHasherPipelineStage nextStage)
            : this(nextStage, Environment.ProcessorCount)
        { }

        // Dispose.
        public void Dispose()
        {
            nextStage.Dispose();
            semaphore.Dispose();
        }
        
        // Properties.
        public bool IsUsable { get; private set; } = true;

        // Methods.
        public async Task<SwarmChunkReference> HashDataAsync(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            using var memoryStream = new MemoryStream(data);
            return await HashDataAsync(memoryStream).ConfigureAwait(false);
        }
        
        public async Task<SwarmChunkReference> HashDataAsync(Stream dataStream)
        {
            ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));

            if (!IsUsable)
                throw new InvalidOperationException("Pipeline has already been used");
            
            // Make it no more usable.
            IsUsable = false;
            
            // Slicing the stream permits to avoid to load all the stream in memory at the same time.
            var chunkBuffer = new byte[SwarmChunk.DataSize];
            int chunkReadSize;
            do
            {
                chunkReadSize = await dataStream.ReadAsync(chunkBuffer).ConfigureAwait(false);
                
                if (chunkReadSize > 0 || //write only chunks with data
                    passedBytes == 0)    //or if the first and only one is empty
                {
                    // Copy read data from buffer to a new chunk data byte[]. Include also span
                    var chunkData = new byte[SwarmChunk.SpanSize + chunkReadSize];
                    chunkBuffer.AsSpan(0, chunkReadSize).CopyTo(chunkData.AsSpan(SwarmChunk.SpanSize));
                    
                    // Write chunk span.
                    SwarmChunk.WriteSpan(
                        chunkData.AsSpan(0, SwarmChunk.SpanSize),
                        (ulong)chunkReadSize);
                
                    // Invoke next stage with parallelism on chunks.
                    await semaphore.WaitAsync().ConfigureAwait(false);
                    var feedArgs = new HasherPipelineFeedArgs(
                        span: chunkData[..SwarmChunk.SpanSize],
                        data: chunkData,
                        numberId: passedBytes / SwarmChunk.DataSize);
                    nextStageTasks.Add(
                        Task.Run(async () =>
                        {
                            try
                            {
                                await nextStage.FeedAsync(feedArgs).ConfigureAwait(false);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                    
                    passedBytes += chunkReadSize;
                }
            } while (chunkReadSize == SwarmChunk.DataSize);

            // Wait the end of all chunk computation.
            await Task.WhenAll(nextStageTasks).ConfigureAwait(false);

            return await nextStage.SumAsync().ConfigureAwait(false);
        }
    }
}