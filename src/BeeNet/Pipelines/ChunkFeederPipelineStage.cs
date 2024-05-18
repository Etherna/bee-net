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
using System.Buffers.Binary;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    /// <summary>
    /// Produce chunked data with span prefix for successive stages
    /// </summary>
    internal class ChunkFeederPipelineStage : PipelineStageBase
    {
        // Fields.
        private readonly byte[] buffer;
        private int bufferIndex;
        private long wroteBytes;
        
        // Constructor.
        public ChunkFeederPipelineStage(PipelineStageBase nextStage)
            : base(nextStage)
        {
            buffer = new byte[SwarmChunk.Size];
        }
        
        // Methods.
        /// <summary>
        /// Produces data chunks for next stages, and returns the number of bytes written to the feeder.
        /// The number of bytes written does not necessarily reflect how many bytes were actually flushed to
        /// subsequent writers, since the feeder is buffered and works in chunk-size quantiles.
        /// </summary>
        /// <param name="args">Pipeline args</param>
        public override async Task FeedAsync(PipelineFeedArgs args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));
            
            // If new data can be fully buffered without complete a chunk, simply add to buffer and return.
            if (args.Data.Length < SwarmChunk.Size - bufferIndex)
            {
                args.Data.CopyTo(buffer.AsSpan(bufferIndex));
                bufferIndex += args.Data.Length;
                return;
            }
            
            // Else, if a new chunk is required, create it starting with buffer content.
            var chunkData = new byte[SwarmChunk.ChunkWithSpanSize];
            Array.Copy(buffer, 0, chunkData, SwarmChunk.SpanSize, bufferIndex);
            
            // Consume input data.
            for (var i = 0; i < args.Data.Length;)
            {
                // At this point the chunk can always be fulfilled because of conditions.
                // Fill the new chunk with data from source.
                var fillingDataLength = SwarmChunk.Size - bufferIndex;
                args.Data[i..(i + fillingDataLength)]
                    .CopyTo(chunkData.AsSpan(SwarmChunk.SpanSize + bufferIndex));
                i += fillingDataLength;
                
                // Write chunk span.
                BinaryPrimitives.WriteUInt64LittleEndian(
                    chunkData.AsSpan(0, SwarmChunk.SpanSize),
                    SwarmChunk.Size);
                
                // Invoke next stage.
                await FeedNextAsync(new PipelineFeedArgs(
                    data: chunkData,
                    span: chunkData[..SwarmChunk.SpanSize])).ConfigureAwait(false);

                bufferIndex = 0;
                wroteBytes += SwarmChunk.Size;
                
                // If we can't fill a whole new chunk, buffer the remaining data and exit.
                if (args.Data.Length - i < SwarmChunk.Size)
                {
                    args.Data[i..].CopyTo(buffer);
                    bufferIndex = args.Data.Length - i;
                    break; //"i += bufferIndex" is same as "i = args.Data.Length"
                }
            }
        }
        
        /// <summary>
        /// Sum flushes any pending data to subsequent writers and returns
        /// the cryptographic root-hash respresenting the data written to
        /// the feeder.
        /// </summary>
        /// <returns>Cryptographic root-hash respresenting the data written</returns>
        public override async Task<byte[]> SumAsync()
        {
            // flush existing data in the buffer
            if (bufferIndex > 0)
            {
                var d = new byte[bufferIndex + SwarmChunk.SpanSize];
                
                int minLength = Math.Min(buffer.Length - bufferIndex, bufferIndex);
                Array.Copy(buffer, bufferIndex, d, SwarmChunk.SpanSize, minLength);
                
                byte[] subArrayD = new byte[SwarmChunk.SpanSize];
                BinaryPrimitives.WriteUInt64LittleEndian(subArrayD, (ulong)bufferIndex);
                Array.Copy(subArrayD, 0, d, 0, SwarmChunk.SpanSize);

                var args = new PipelineFeedArgs(
                    data: d,
                    span: d[..SwarmChunk.SpanSize]);
                await FeedNextAsync(args).ConfigureAwait(false);
                wroteBytes += d.Length;
            }

            if (wroteBytes == 0)
            {
                // this is an empty file, we should write the span of
                // an empty file (0).
                var d = new byte[SwarmChunk.SpanSize];
                var args = new PipelineFeedArgs(
                    data: d,
                    span: d);
                await FeedNextAsync(args).ConfigureAwait(false);
                wroteBytes += d.Length;
            }

            return await SumNextAsync().ConfigureAwait(false);
        }
    }
}