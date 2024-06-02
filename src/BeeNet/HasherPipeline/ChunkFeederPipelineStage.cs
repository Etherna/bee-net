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

namespace Etherna.BeeNet.HasherPipeline
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
        private readonly byte[] buffer = new byte[SwarmChunk.DataSize];
        private readonly List<Task> chunkTasks = new();
        private readonly SemaphoreSlim semaphore = new(chunkConcurrency, chunkConcurrency);
        
        private int bufferIndex;
        private long wroteBytes;
        
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
            
            // Slicing the stream permits to avoid to load all the stream in memory at the same time.
            var chunkData = new byte[SwarmChunk.DataSize];
            int chunkReadBytes;
            do
            {
                chunkReadBytes = await dataStream.ReadAsync(chunkData).ConfigureAwait(false);
                if (chunkReadBytes > 0)
                    await FeedDataAsync(chunkData.AsMemory()[..chunkReadBytes]).ConfigureAwait(false);
            } while (chunkReadBytes == SwarmChunk.DataSize);

            // Wait the end of all chunk computation.
            await Task.WhenAll(chunkTasks).ConfigureAwait(false);

            return await SumAsync().ConfigureAwait(false);
        }

        // Helper methods.
        /// <summary>
        /// Produces data chunks for next stages.
        /// </summary>
        /// <param name="args">Pipeline args</param>
        private async Task FeedDataAsync(Memory<byte> data)
        {
            // If new data can be fully buffered without complete a chunk, simply add to buffer and return.
            if (data.Length < SwarmChunk.DataSize - bufferIndex)
            {
                data.CopyTo(buffer.AsMemory(bufferIndex));
                bufferIndex += data.Length;
                return;
            }
            
            // Else, if a new chunk is required, create it starting with buffer content.
            var chunkData = new byte[SwarmChunk.SpanAndDataSize];
            Array.Copy(buffer, 0, chunkData, SwarmChunk.SpanSize, bufferIndex);
            
            // Consume input data.
            for (var i = 0; i < data.Length;)
            {
                // At this point the chunk can always be fulfilled because of conditions.
                // Fill the new chunk with data from source.
                var fillingDataLength = SwarmChunk.DataSize - bufferIndex;
                data[i..(i + fillingDataLength)]
                    .CopyTo(chunkData.AsMemory(SwarmChunk.SpanSize + bufferIndex));
                i += fillingDataLength;
                
                // Write chunk span.
                SwarmChunk.WriteSpan(
                    chunkData.AsSpan(0, SwarmChunk.SpanSize),
                    SwarmChunk.DataSize);
                
                // Invoke next stage with parallelism on chunks.
                await semaphore.WaitAsync().ConfigureAwait(false);
                var feedArgs = new HasherPipelineFeedArgs(
                    span: chunkData[..SwarmChunk.SpanSize],
                    data: chunkData,
                    numberId: wroteBytes / SwarmChunk.DataSize);
                chunkTasks.Add(
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

                bufferIndex = 0;
                wroteBytes += SwarmChunk.DataSize;
                
                // If we can't fill a whole new chunk, buffer the remaining data and exit.
                if (data.Length - i < SwarmChunk.DataSize)
                {
                    data[i..].CopyTo(buffer);
                    bufferIndex = data.Length - i;
                    break; //"i += bufferIndex" is same as "i = args.Data.Length"
                }
            }
        }
        
        /// <summary>
        /// Flushes any pending data to subsequent writers and returns the cryptographic root-hash
        /// respresenting the data written to the feeder.
        /// </summary>
        /// <returns>Cryptographic root-hash representing the data written</returns>
        private async Task<SwarmAddress> SumAsync()
        {
            if (bufferIndex > 0 || //if we need to flush existing data from the buffer,
                wroteBytes == 0)   //or if no chunks have been written at all
            {
                var chunkData = new byte[SwarmChunk.SpanSize + bufferIndex];
                Array.Copy(buffer, 0, chunkData, SwarmChunk.SpanSize, bufferIndex);
                
                // Write chunk span.
                SwarmChunk.WriteSpan(
                    chunkData.AsSpan(0, SwarmChunk.SpanSize),
                    (ulong)bufferIndex);

                // Invoke next stage.
                await nextStage.FeedAsync(new HasherPipelineFeedArgs(
                    span: chunkData[..SwarmChunk.SpanSize],
                    data: chunkData,
                    numberId: wroteBytes / SwarmChunk.DataSize)).ConfigureAwait(false);
            }

            return await nextStage.SumAsync().ConfigureAwait(false);
        }
    }
}