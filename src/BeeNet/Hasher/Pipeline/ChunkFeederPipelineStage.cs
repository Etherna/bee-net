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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Pipeline
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
        public async Task<SwarmAddress> HashDataAsync(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            using var memoryStream = new MemoryStream(data);
            return await HashDataAsync(memoryStream).ConfigureAwait(false);
        }
        
        public async Task<SwarmAddress> HashDataAsync(Stream dataStream)
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