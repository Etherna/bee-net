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
             * Double semaphores compared to current chunk concurrency.
             * This avoids the race condition when: a chunk complete its hashing, it's semaphore is assigned and
             * locked by another one, and only after this the direct child of the first one tries to wait its parent.
             *
             * This is possible because the returning order of semaphores in queue is not guaranteed.
             *
             * Explanation:
             * While the task of chunk A is still waiting to lock on its prev A-1, all the prev chunks have ended tasks.
             * Anyway, prevs didn't end in order, and for some reason semaphore that was of chunk A-1 comes in order
             * before than next "Concurrency -1" (only active task is with A). Because of this, it can be allocated
             * with any next task from A+1. If this happens before A locks on semaphore of A-1, we are in deadlock.
             *
             * Instead, doubling semaphores we guarantee that queue never goes under level of concurrency
             * with contained elements, so a prev chunk's semaphore can't be reused until it's direct next
             * has completed and released concurrency.
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

        public long MissedOptimisticHashing => nextStage.MissedOptimisticHashing;

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
                    var chunkNumberId = passedBytes / SwarmChunk.DataSize;
                    
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
                        numberId: chunkNumberId,
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