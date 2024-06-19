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
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Pipeline
{
    /// <summary>
    /// Produce chunked data with span prefix for successive stages.
    /// Also controls the parallelism on chunk elaboration
    /// </summary>
    internal sealed class ChunkFeederPipelineStage : IHasherPipeline
    {
        // Fields.
        private readonly List<Task> nextStageTasks = new();
        private readonly SemaphoreSlim semaphore;
        
        private long passedBytes;
        private readonly bool useCompaction;
        private readonly IHasherPipelineStage nextStage;

        // Constructor.
        public ChunkFeederPipelineStage(
            bool useCompaction,
            IHasherPipelineStage nextStage)
            : this(useCompaction, nextStage, Environment.ProcessorCount)
        { }

        public ChunkFeederPipelineStage(
            bool useCompaction,
            IHasherPipelineStage nextStage,
            int chunkConcurrency)
        {
            this.useCompaction = useCompaction;
            this.nextStage = nextStage;
            semaphore = new(chunkConcurrency, chunkConcurrency);
        }

        // Dispose.
        public void Dispose()
        {
            nextStage.Dispose();
            semaphore.Dispose();
        }
        
        // Properties.
        public bool IsUsable { get; private set; } = true;

        // Methods.
        public async Task<SwarmHash> HashDataAsync(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            using var memoryStream = new MemoryStream(data);
            return await HashDataAsync(memoryStream).ConfigureAwait(false);
        }
        
        public async Task<SwarmHash> HashDataAsync(Stream dataStream)
        {
            ArgumentNullException.ThrowIfNull(dataStream, nameof(dataStream));

            if (!IsUsable)
                throw new InvalidOperationException("Pipeline has already been used");
            
            // Make it no more usable.
            IsUsable = false;
            
            // If we are using compaction, use first data byte for the seed.
            var chunkDataSize = SwarmChunk.DataSize;
            if (useCompaction)
                chunkDataSize--;
            
            // Slicing the stream permits to avoid to load all the stream in memory at the same time.
            var chunkBuffer = new byte[chunkDataSize];
            int chunkReadSize;
            do
            {
                chunkReadSize = await dataStream.ReadAsync(chunkBuffer).ConfigureAwait(false);
                
                if (chunkReadSize > 0 || //write only chunks with data
                    passedBytes == 0)    //or if the first and only one is empty
                {
                    // Copy read data from buffer to a new chunk data byte[]. Include also span
                    var chunkData = useCompaction
                        ? new byte[SwarmChunk.SpanSize + chunkReadSize + 1]
                        : new byte[SwarmChunk.SpanSize + chunkReadSize];
                    chunkBuffer.AsSpan(0, chunkReadSize).CopyTo(
                        chunkData.AsSpan(useCompaction
                            ? SwarmChunk.SpanSize + 1
                            : SwarmChunk.SpanSize));

                    // If we are using compaction, initialize the seed byte with a random number.
                    if (useCompaction)
                        chunkData[SwarmChunk.SpanSize] = RandomNumberGenerator.GetBytes(1)[0];
                    
                    // Write chunk span.
                    SwarmChunk.WriteSpan(
                        chunkData.AsSpan(0, SwarmChunk.SpanSize),
                        useCompaction
                            ? (ulong)chunkReadSize + 1
                            : (ulong)chunkReadSize);
                
                    // Invoke next stage with parallelism on chunks.
                    await semaphore.WaitAsync().ConfigureAwait(false);
                    var feedArgs = new HasherPipelineFeedArgs(
                        span: chunkData[..SwarmChunk.SpanSize],
                        data: chunkData,
                        numberId: passedBytes / chunkDataSize);
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
            } while (chunkReadSize == chunkDataSize);

            // Wait the end of all chunk computation.
            await Task.WhenAll(nextStageTasks).ConfigureAwait(false);

            return await nextStage.SumAsync().ConfigureAwait(false);
        }
    }
}