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
    internal class ChunkFeederPipelineStage : PipelineStageBase
    {
        // Fields.
        private byte[] buffer;
        private int bufferIdx;
        private long wrote;
        
        // Constructor.
        public ChunkFeederPipelineStage(PipelineStageBase next)
            : base(next)
        {
            buffer = new byte[SwarmChunk.Size];
        }
        
        // Methods.
        /// <summary>
        /// Writes data to the chunk feeder. It returns the number of bytes written
        /// to the feeder. The number of bytes written does not necessarily reflect how many
        /// bytes were actually flushed to subsequent writers, since the feeder is buffered
        /// and works in chunk-size quantiles.
        /// </summary>
        /// <param name="context"></param>
        public override async Task<int> FeedAsync(PipelineFeedContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            if (Next is null)
                throw new InvalidOperationException("Next stage can't be null here");

            var data = context.Data.ToArray();
            
            var w = 0;
            if (data.Length + bufferIdx < SwarmChunk.Size)
            {
                // write the data into the buffer and return
                Array.Copy(data, 0, buffer, bufferIdx, data.Length);
                bufferIdx += data.Length;
                return data.Length;
            }

            // if we are here it means we have to do at least one write
            var d = new byte[SwarmChunk.ChunkWithSpanSize];

            //copy from existing buffer to this one
            Array.Copy(buffer, 0, d, SwarmChunk.SpanSize, bufferIdx);
            int sp = bufferIdx; // span of current write

            // don't account what was already in the buffer when returning
            // number of written bytes
            if (sp > 0)
                w -= sp;

            int n;
            for (var i = 0; i < data.Length;)
            {
                // if we can't fill a whole write, buffer the rest and return
                if (sp + (data.Length - i) < SwarmChunk.Size)
                {
                    Array.Copy(data, i, buffer, 0, data.Length - i);
                    bufferIdx = data.Length - i;
                    return w + data.Length - i;
                }

                // fill stuff up from the incoming write
                n = Math.Min(data.Length - i, d.Length - (SwarmChunk.SpanSize + bufferIdx));
                Array.Copy(data, i, d, SwarmChunk.SpanSize + bufferIdx, n);
                i += n;
                sp += n;
                
                byte[] subArrayD = new byte[SwarmChunk.SpanSize];
                BinaryPrimitives.WriteUInt64LittleEndian(subArrayD, (ulong)sp);
                Array.Copy(subArrayD, 0, d, 0, SwarmChunk.SpanSize);

                var args = new PipelineFeedContext(d[..(SwarmChunk.SpanSize + sp)])
                {
                    Span = d[..SwarmChunk.SpanSize]
                };
                await Next.FeedAsync(args).ConfigureAwait(false);
                bufferIdx = 0;
                w += sp;
                sp = 0;
            }

            wrote += w;
            return w;
        }
        
        /// <summary>
        /// Sum flushes any pending data to subsequent writers and returns
        /// the cryptographic root-hash respresenting the data written to
        /// the feeder.
        /// </summary>
        /// <returns>Cryptographic root-hash respresenting the data written</returns>
        public override async Task<byte[]> SumAsync()
        {
            if (Next is null)
                throw new InvalidOperationException("Next stage can't be null here");
            
            // flush existing data in the buffer
            if (bufferIdx > 0)
            {
                var d = new byte[bufferIdx + SwarmChunk.SpanSize];
                
                int minLength = Math.Min(buffer.Length - bufferIdx, bufferIdx);
                Array.Copy(buffer, bufferIdx, d, SwarmChunk.SpanSize, minLength);
                
                byte[] subArrayD = new byte[SwarmChunk.SpanSize];
                BinaryPrimitives.WriteUInt64LittleEndian(subArrayD, (ulong)bufferIdx);
                Array.Copy(subArrayD, 0, d, 0, SwarmChunk.SpanSize);

                var args = new PipelineFeedContext(d)
                {
                    Span = d[..SwarmChunk.SpanSize]
                };
                await Next.FeedAsync(args).ConfigureAwait(false);
                wrote += d.Length;
            }

            if (wrote == 0)
            {
                // this is an empty file, we should write the span of
                // an empty file (0).
                var d = new byte[SwarmChunk.SpanSize];
                var args = new PipelineFeedContext(d)
                {
                    Span = d
                };
                await Next.FeedAsync(args).ConfigureAwait(false);
                wrote += d.Length;
            }

            return await Next.SumAsync().ConfigureAwait(false);
        }
    }
}