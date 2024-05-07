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

namespace Etherna.BeeNet.Services.Pipelines
{
    public class ChunkFeeder
    {
        // Fields.
        private byte[] buffer;
        private int bufferIdx;
        private PipelineStageBase next;
        private long wrote;
        
        // Constructor.
        public ChunkFeeder(PipelineStageBase next)
        {
            this.next = next;
            buffer = new byte[SwarmChunk.Size];
        }
        
        // Methods.
        /// <summary>
        /// Sum flushes any pending data to subsequent writers and returns
        /// the cryptographic root-hash respresenting the data written to
        /// the feeder.
        /// </summary>
        /// <returns></returns>
        public byte[] Sum()
        {
            // flush existing data in the buffer
            if (bufferIdx > 0)
            {
                var d = new byte[bufferIdx + SwarmChunk.SpanSize];
                
                int minLength = Math.Min(buffer.Length - bufferIdx, bufferIdx);
                Array.Copy(buffer, bufferIdx, d, SwarmChunk.SpanSize, minLength);
                
                byte[] subArrayD = new byte[SwarmChunk.SpanSize];
                BinaryPrimitives.WriteUInt64LittleEndian(subArrayD, (ulong)bufferIdx);
                Array.Copy(subArrayD, 0, d, 0, SwarmChunk.SpanSize);

                var args = new PipelineWriteContext
                {
                    Data = d,
                    Span = d[..SwarmChunk.SpanSize]
                };
                next.ChainWrite(args);
                wrote += d.Length;
            }

            if (wrote == 0)
            {
                // this is an empty file, we should write the span of
                // an empty file (0).
                var d = new byte[SwarmChunk.SpanSize];
                var args = new PipelineWriteContext
                {
                    Data = d,
                    Span = d
                };
                next.ChainWrite(args);
                wrote += d.Length;
            }

            return next.Sum();
        }
        
        /// <summary>
        /// Write writes data to the chunk feeder. It returns the number of bytes written
        /// to the feeder. The number of bytes written does not necessarily reflect how many
        /// bytes were actually flushed to subsequent writers, since the feeder is buffered
        /// and works in chunk-size quantiles.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int Write(byte[] b)
        {
            ArgumentNullException.ThrowIfNull(b, nameof(b));
            
            var l = b.Length; // data length
            var w = 0; // written

            if (l + bufferIdx < SwarmChunk.Size)
            {
                // write the data into the buffer and return
                Array.Copy(b, 0, buffer, bufferIdx, b.Length);
                bufferIdx += b.Length;
                return b.Length;
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
            for (var i = 0; i < b.Length;)
            {
                // if we can't fill a whole write, buffer the rest and return
                if (sp + (b.Length - i) < SwarmChunk.Size)
                {
                    Array.Copy(b, i, buffer, 0, b.Length - i);
                    bufferIdx = b.Length - i;
                    return w + b.Length - i;
                }

                // fill stuff up from the incoming write
                n = Math.Min(b.Length - i, d.Length - (SwarmChunk.SpanSize + bufferIdx));
                Array.Copy(b, i, d, SwarmChunk.SpanSize + bufferIdx, n);
                i += n;
                sp += n;
                
                byte[] subArrayD = new byte[SwarmChunk.SpanSize];
                BinaryPrimitives.WriteUInt64LittleEndian(subArrayD, (ulong)sp);
                Array.Copy(subArrayD, 0, d, 0, SwarmChunk.SpanSize);

                var args = new PipelineWriteContext
                {
                    Data = d[..(SwarmChunk.SpanSize + sp)],
                    Span = d[..SwarmChunk.SpanSize]
                };
                next.ChainWrite(args);
                bufferIdx = 0;
                w += sp;
                sp = 0;
            }

            wrote += w;
            return w;
        }
    }
}