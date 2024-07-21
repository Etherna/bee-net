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

namespace Etherna.BeeNet.Hashing.Pipeline
{
    /// <summary>
    /// Produce chunked data with span prefix for successive stages.
    /// Also controls the parallelism on chunk elaboration
    /// </summary>
    internal sealed class ChunkFeederPipelineStage : IHasherPipeline
    {
        // Fields.
        private readonly SemaphoreSlim chunkConcurrencySemaphore;
        private readonly ConcurrentQueue<SemaphoreSlim> chunkSemaphorePool;
        private readonly IHasherPipelineStage nextStage;
        private readonly List<Task> nextStageTasks = new();
        
        private long passedBytes;

        // Constructors.
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
        public ChunkFeederPipelineStage(
            IHasherPipelineStage nextStage,
            int? chunkConcurrency = default)
        {
            chunkConcurrency ??= Environment.ProcessorCount;
            
            this.nextStage = nextStage;
            chunkConcurrencySemaphore = new(chunkConcurrency.Value, chunkConcurrency.Value);
            chunkSemaphorePool = new ConcurrentQueue<SemaphoreSlim>();
            
            //init semaphore pool
            /*
             * Duplicate number of semaphore respect to level of concurrency.
             * This avoids the possible race condition where a chunk starts to wait a prev chunk,
             * after that the prev chunk's semaphore is already been assigned to a new next chunk.
             *
             * In this way, because semaphores are distributed with a queue, the queue will never reassign
             * a semaphore before the chunk windows has shifted, and prev chunk linking it has completed.
             */
            for (int i = 0; i < chunkConcurrency * 2; i++)
                chunkSemaphorePool.Enqueue(new SemaphoreSlim(1, 1));
        }

        // Dispose.
        public void Dispose()
        {
            nextStage.Dispose();
            chunkConcurrencySemaphore.Dispose();
            while (chunkSemaphorePool.TryDequeue(out var semaphore))
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
            SemaphoreSlim? prevChunkSemaphore = null;
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
                    //control concurrency
                    await chunkConcurrencySemaphore.WaitAsync().ConfigureAwait(false);
                    
                    //initialize chunk semaphore, receiving from semaphore pool
#pragma warning disable CA2000
                    if (!chunkSemaphorePool.TryDequeue(out var chunkSemaphore))
                        throw new InvalidOperationException("Semaphore pool exhausted");
#pragma warning restore CA2000
                    await chunkSemaphore.WaitAsync().ConfigureAwait(false);
                    
                    //build args
                    var feedArgs = new HasherPipelineFeedArgs(
                        span: chunkData[..SwarmChunk.SpanSize],
                        data: chunkData,
                        numberId: passedBytes / SwarmChunk.DataSize,
                        prevChunkSemaphore: prevChunkSemaphore);
                    
                    //run task
                    nextStageTasks.Add(
                        Task.Run(async () =>
                        {
                            try
                            {
                                await nextStage.FeedAsync(feedArgs).ConfigureAwait(false);
                            }
                            finally
                            {
                                //release and restore chunk semaphore in pool
                                chunkSemaphore.Release();
                                chunkSemaphorePool.Enqueue(chunkSemaphore);
                                
                                //release task for next chunk
                                chunkConcurrencySemaphore.Release();
                            }
                        }));
                    
                    //set current chunk semaphore as prev for next chunk
                    prevChunkSemaphore = chunkSemaphore;
                    
                    passedBytes += chunkReadSize;
                }
            } while (chunkReadSize == SwarmChunk.DataSize);

            // Wait the end of all chunk computation.
            await Task.WhenAll(nextStageTasks).ConfigureAwait(false);

            return await nextStage.SumAsync().ConfigureAwait(false);
        }
    }
}