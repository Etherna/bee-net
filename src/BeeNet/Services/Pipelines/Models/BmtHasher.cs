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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services.Pipelines.Models
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class BmtHasher
    {
        // Constructor.
        public BmtHasher(BmtPoolConfig config, BmtTree bmt)
        {
            Bmt = bmt;
            Config = config;
            Result = new ConcurrentQueue<byte[]>();
            Span = new byte[SwarmChunk.SpanSize];
        }

        // Properties.
        /// <summary>
        /// Prebuilt BMT resource for flow control and proofs
        /// </summary>
        public BmtTree Bmt { get; }
        
        public BmtPoolConfig Config { get; }
        
        /// <summary>
        /// Offset (cursor position) within currently open segment
        /// </summary>
        public int Offset { get; private set; }
        
        /// <summary>
        /// Index of rightmost currently open segment
        /// </summary>
        public int Pos { get; private set; }
        
        public ConcurrentQueue<byte[]> Result { get; }
        
        /// <summary>
        /// Bytes written to Hasher since last Reset()
        /// </summary>
        public int Size { get; private set; }
        
        /// <summary>
        /// The span of the data subsumed under the chunk
        /// </summary>
        public byte[] Span { get; }
 
        // Methods.
        public byte[] Hash(byte[]? b)
        {
            if (Size == 0)
                return Config.Hasher(Span.Concat(Config.Zerohashes[Config.Depth]).ToArray());
            
            copy(h.bmt.buffer[h.size:], zerosection)
            // write the last section with final flag set to true
            go h.processSection(h.pos, true)
            select {
                case result := <-h.result:
                return doHash(h.hasher(), h.span, result)
                case err := <-h.errc:
                return nil, err
            }
        }
        
        public void SetHeader(byte[] span)
        {
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            span.CopyTo(Span, 0);
        }

        public int Write(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
            
            var length = bytes.Length;
            var max = Config.MaxSize - Size;
            length = Math.Min(length, max);
            bytes.CopyTo(Bmt.Buffer, Size);
            var secsize = 2 * Config.SegmentSize;
            var from = Size / secsize;
            Offset = Size % secsize;
            Size += length;
            var to = Size / secsize;
            if (length == max)
                to--;
            Pos = to;
            var tasks = new List<Task>();
            for (var i = from; i < to; i++)
                tasks.Add(Task.Run(() => ProcessSection(i, false)));
            Task.WhenAll(tasks).Wait();
            return length;
        }
        
        // Helpers.
        /// <summary>
        /// Writes the hash of i-th section into level 1 node of the BMT tree.
        /// </summary>
        private void ProcessSection(int i, bool final)
        {
            var secsize = 2 * Config.SegmentSize;
            var offset = i * secsize;
            var level = 1;

            // select the leaf node for the section
            var n = Bmt.Leaves[i];
            var isLeft = n.IsLeft;
            var hasher = Config.Hasher;
            n = n.Parent!;

            // hash the section
            var section = hasher(Bmt.Buffer[Offset..(Offset + secsize)]);

            // write hash into parent node
            if (final)
                WriteFinalNode(level, n, isLeft, section); //for the last segment use writeFinalNode
            else
                WriteNode(n, isLeft, section);
        }

        /// <summary>
        /// WriteNode pushes the data to the node.
        /// If it is the first of 2 sisters written, the routine terminates.
        /// If it is the second, it calculates the hash and writes it to the parent node recursively.
        /// Since hashing the parent is synchronous the same hasher can be used.
        /// </summary>
        private void WriteNode(BmtTreeNode? n, bool isLeft, byte[] s)
        {
            var level = 1;
            while (true)
            {
                // at the root of the bmt just write the result to the result channel
                if (n is null)
                {
                    Result.Enqueue(s);
                    return;
                }

                // otherwise assign child hash to left or right segment
                if (isLeft)
                {
                    n.Left = s;
                } else
                {
                    n.Right = s;
                }
                
                // the child-thread first arriving will terminate
                if (n.Toggle())
                    return;
                
                // the thread coming second now can be sure both left and right children are written
                // so it calculates the hash of left|right and pushes it to the parent
                s = Config.Hasher(n.Left!.Concat(n.Right!).ToArray());
                isLeft = n.IsLeft;
                n = n.Parent;
                level++;
            }
        }

        /// <summary>
        /// WriteFinalNode is following the path starting from the final datasegment to the BMT root via parents.
        /// For unbalanced trees it fills in the missing right sister nodes using the pool's lookup table for BMT subtree root hashes for all-zero sections.
        /// Otherwise behaves like `writeNode`.
        /// </summary>
        private void WriteFinalNode(int level, BmtTreeNode? n, bool isLeft, byte[]? s)
        {
            while (true)
            {
                // at the root of the bmt just write the result to the result channel
                if (n is null)
                {
                    Result.Enqueue(s!);
                    return;
                }

                bool noHash;
                if (isLeft)
                {
                    // coming from left sister branch
                    // when the final section's path is going via left child node
                    // we include an all-zero subtree hash for the right level and toggle the node.
                    n.Right = Config.Zerohashes[level];
                    if(s is not null)
                    {
                        n.Left = s;
                        // if a left final node carries a hash, it must be the first (and only thread)
                        // so the toggle is already in passive state no need no call
                        // yet thread needs to carry on pushing hash to parent
                        noHash = false;
                    }
                    else
                    {
                        // if again first thread then propagate nil and calculate no hash
                        noHash = n.Toggle();
                    }
                }
                else
                {
                    // right sister branch
                    if (s is not null)
                    {
                        // if hash was pushed from right child node, write right segment change state
                        n.Right = s;
                        // if toggle is true, we arrived first so no hashing just push nil to parent
                        noHash = n.Toggle();
                    }
                    else
                    {
                        // if s is nil, then thread arrived first at previous node and here there will be two,
                        // so no need to do anything and keep s = nil for parent
                        noHash = true;
                    }
                }
                // the child-thread first arriving will just continue resetting s to nil
                // the second thread now can be sure both left and right children are written
                // it calculates the hash of left|right and pushes it to the parent
                if (noHash)
                {
                    s = null;
                }
                else
                {
                    s = Config.Hasher(n.Left!.Concat(n.Right!).ToArray());
                }
                
                // iterate to parent
                isLeft = n.IsLeft;
                n = n.Parent;
                level++;
            }
        }
    }
}