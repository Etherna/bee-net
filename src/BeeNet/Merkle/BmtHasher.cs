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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Etherna.BeeNet.Merkle
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    internal class BmtHasher
    {
        private readonly byte[] zerospan;
        private readonly byte[] zerosection;
        private byte[] _span;

        // Constructor.
        public BmtHasher(Func<byte[], byte[]> hasherFunc, int segmentCount)
        {
            ArgumentNullException.ThrowIfNull(hasherFunc, nameof(hasherFunc));
            
            // Define BMT depth and segment count.
            Depth = 1;
            SegmentCount = 2;
            while (SegmentCount < segmentCount)
            {
                Depth++;
                SegmentCount *= 2;
            }
            
            // Initialize properties.
            HasherFunc = hasherFunc;
            SegmentSize = hasherFunc(Array.Empty<byte>()).Length;
            _span = new byte[SwarmChunk.SpanSize];
            zerospan = new byte[8];
            zerosection = new byte[64];
            
            // Initialize the ZeroHashes lookup table.
            ZeroHashes = new byte[Depth + 1][];
            var zerosLayer = new byte[SegmentSize];
            ZeroHashes[0] = zerosLayer;
            for (int i = 1; i < Depth + 1; i++)
            {
                zerosLayer = hasherFunc(zerosLayer.Concat(zerosLayer).ToArray());
                ZeroHashes[i] = zerosLayer;
            }

            // Initialize BMT.
            Bmt = new Bmt(MaxSize, Depth, hasherFunc);
        }

        // Properties.
        /// <summary>
        /// Prebuilt BMT resource for flow control and proofs
        /// </summary>
        public Bmt Bmt { get; }
        
        /// <summary>
        /// Depth of the bmt trees = (int)(log2(segmentCount))+1
        /// </summary>
        public int Depth { get; }
        
        /// <summary>
        /// Hashing function
        /// </summary>
        public Func<byte[],byte[]> HasherFunc { get; }

        /// <summary>
        /// The total length of the data (SegmentCount * SegmentSize)
        /// </summary>
        public int MaxSize => SegmentCount * SegmentSize;
        
        /// <summary>
        /// Offset (cursor position) within currently open segment
        /// </summary>
        public int Offset { get; private set; }
        
        /// <summary>
        /// Index of rightmost currently open segment
        /// </summary>
        public int Pos { get; private set; }
        
        public byte[]? Result { get; private set; }
        
        /// <summary>
        /// The number of segments on the base level of the BMT
        /// </summary>
        public int SegmentCount { get; }
        
        /// <summary>
        /// Size of leaf segments, stipulated to be = hash size
        /// </summary>
        public int SegmentSize { get; }
        
        /// <summary>
        /// Bytes written to Hasher since last Reset()
        /// </summary>
        public int Size { get; private set; }
        
        /// <summary>
        /// The span of the data subsumed under the chunk
        /// </summary>
        public ReadOnlySpan<byte> Span
        {
            get => _span;
            set => _span = value.ToArray();
        }
        
        /// <summary>
        /// Lookup table for predictable padding subtrees for all levels
        /// </summary>
        public byte[][] ZeroHashes { get; }
 
        // Methods.
        public byte[] Hash()
        {
            if (Size == 0)
                return HasherFunc(_span.Concat(ZeroHashes[Depth]).ToArray());
            
            Array.Copy(zerosection, Bmt.Buffer, Size);
            
            // write the last section with final flag set to true
            ProcessSection(Pos, true);
            return HasherFunc(_span.Concat(Result!).ToArray());
        }

        public void Write(ReadOnlySpan<byte> bytes)
        {
            var length = bytes.Length;
            var max = MaxSize - Size;
            length = Math.Min(length, max);
            bytes.CopyTo(Bmt.Buffer.AsSpan(Size));
            var secsize = 2 * SegmentSize;
            var from = Size / secsize;
            Offset = Size % secsize;
            Size += length;
            var to = Size / secsize;
            if (length == max)
                to--;
            Pos = to;
            for (var i = from; i < to; i++)
                ProcessSection(i, false);
        }
        
        // Helpers.
        /// <summary>
        /// Writes the hash of i-th section into level 1 node of the BMT tree.
        /// </summary>
        private void ProcessSection(int i, bool final)
        {
            var secsize = 2 * SegmentSize;
            var offset = i * secsize;
            var level = 1;

            // select the leaf node for the section
            var n = Bmt.Leaves[i];
            var isLeft = n.IsLeft;
            var hasher = HasherFunc;
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
        private void WriteNode(BmtNode? n, bool isLeft, byte[] s)
        {
            var level = 1;
            while (true)
            {
                // at the root of the bmt just write the result to the result channel
                if (n is null)
                {
                    Result = s;
                    return;
                }

                // otherwise assign child hash to left or right segment
                if (isLeft)
                    n.Left = s;
                else
                    n.Right = s;
                
                // the child-thread first arriving will terminate
                if (n.Toggle())
                    return;
                
                // the thread coming second now can be sure both left and right children are written
                // so it calculates the hash of left|right and pushes it to the parent
                s = HasherFunc(n.Left!.Concat(n.Right!).ToArray());
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
        private void WriteFinalNode(int level, BmtNode? n, bool isLeft, byte[]? s)
        {
            while (true)
            {
                // at the root of the bmt just write the result to the result channel
                if (n is null)
                {
                    Result = s;
                    return;
                }

                bool noHash;
                if (isLeft)
                {
                    // coming from left sister branch
                    // when the final section's path is going via left child node
                    // we include an all-zero subtree hash for the right level and toggle the node.
                    n.Right = ZeroHashes[level];
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
                    s = HasherFunc(n.Left!.Concat(n.Right!).ToArray());
                }
                
                // iterate to parent
                isLeft = n.IsLeft;
                n = n.Parent;
                level++;
            }
        }
    }
}