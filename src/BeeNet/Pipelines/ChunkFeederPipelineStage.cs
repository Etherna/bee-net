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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    /// <summary>
    /// Produce chunked data with span prefix for successive stages.
    /// Also controls the parallelism on chunk elaboration
    /// </summary>
    internal sealed class ChunkFeederPipelineStage : PipelineStageBase
    {
        // Fields.
        private readonly byte[] buffer;
        // private readonly SemaphoreSlim semaphore;
        
        private int bufferIndex;
        private long wroteBytes;
        
        // Constructor.
        public ChunkFeederPipelineStage(PipelineStageBase nextStage)
            : this(nextStage, Environment.ProcessorCount * 2)
        { }
        
        public ChunkFeederPipelineStage(
            PipelineStageBase nextStage,
            int chunkConcurrency)
            : base(nextStage)
        {
            buffer = new byte[SwarmChunk.DataSize];
            // semaphore = new SemaphoreSlim(chunkConcurrency, chunkConcurrency);
        }
        
        // // Dispose.
        // public void Dispose()
        // {
        //     semaphore.Dispose();
        // }
        
        // Protected methods.
        /// <summary>
        /// Produces data chunks for next stages.
        /// </summary>
        /// <param name="args">Pipeline args</param>
        protected override async Task FeedImplAsync(PipelineFeedArgs args)
        {
            // If new data can be fully buffered without complete a chunk, simply add to buffer and return.
            if (args.Data.Length < SwarmChunk.DataSize - bufferIndex)
            {
                args.Data.CopyTo(buffer.AsMemory(bufferIndex));
                bufferIndex += args.Data.Length;
                return;
            }
            
            // Else, if a new chunk is required, create it starting with buffer content.
            var chunkData = new byte[SwarmChunk.SpanAndDataSize];
            Array.Copy(buffer, 0, chunkData, SwarmChunk.SpanSize, bufferIndex);
            
            // Consume input data.
            // List<Task> nextTasks = new();
            for (var i = 0; i < args.Data.Length;)
            {
                // At this point the chunk can always be fulfilled because of conditions.
                // Fill the new chunk with data from source.
                var fillingDataLength = SwarmChunk.DataSize - bufferIndex;
                args.Data[i..(i + fillingDataLength)]
                    .CopyTo(chunkData.AsMemory(SwarmChunk.SpanSize + bufferIndex));
                i += fillingDataLength;
                
                // Write chunk span.
                SwarmChunk.WriteSpan(
                    chunkData.AsSpan(0, SwarmChunk.SpanSize),
                    SwarmChunk.DataSize);
                
                // // Invoke next stage with parallelism on chunks.
                // await semaphore.WaitAsync().ConfigureAwait(false);
                // nextTasks.Add(
                //     Task.Run(async () =>
                //     {
                //         try
                //         {
                //             await FeedNextAsync(new PipelineFeedArgs(
                //                 data: chunkData,
                //                 span: chunkData[..SwarmChunk.SpanSize])).ConfigureAwait(false);
                //         }
                //         finally
                //         {
                //             semaphore.Release();
                //         }
                //     }));
                
                // Invoke next stage.
                await FeedNextAsync(new PipelineFeedArgs(
                    data: chunkData,
                    span: chunkData[..SwarmChunk.SpanSize])).ConfigureAwait(false);

                bufferIndex = 0;
                wroteBytes += SwarmChunk.DataSize;
                
                // If we can't fill a whole new chunk, buffer the remaining data and exit.
                if (args.Data.Length - i < SwarmChunk.DataSize)
                {
                    args.Data[i..].CopyTo(buffer);
                    bufferIndex = args.Data.Length - i;
                    break; //"i += bufferIndex" is same as "i = args.Data.Length"
                }
            }

            // // Wait the end of all chunk computation.
            // await Task.WhenAll(nextTasks).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Flushes any pending data to subsequent writers and returns the cryptographic root-hash
        /// respresenting the data written to the feeder.
        /// </summary>
        /// <returns>Cryptographic root-hash representing the data written</returns>
        protected override async Task<byte[]> SumImplAsync()
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
                await FeedNextAsync(new PipelineFeedArgs(
                    data: chunkData,
                    span: chunkData[..SwarmChunk.SpanSize])).ConfigureAwait(false);
            }

            return await SumNextAsync().ConfigureAwait(false);
        }
    }
}