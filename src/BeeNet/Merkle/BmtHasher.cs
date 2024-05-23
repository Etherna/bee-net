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
using System.Linq;

namespace Etherna.BeeNet.Merkle
{
    internal class BmtHasher
    {
        /// <summary>
        /// Hashing function
        /// </summary>
        private readonly Func<byte[], byte[]> hasherFunc;
        /// <summary>
        /// Lookup table for predictable padding subtrees for all levels
        /// </summary>
        private readonly byte[][] zeroHashes;
        
        private byte[] _span;

        // Constructor.
        public BmtHasher(Func<byte[], byte[]> hasherFunc, int segmentCount)
        {
            ArgumentNullException.ThrowIfNull(hasherFunc, nameof(hasherFunc));
            
            this.hasherFunc = hasherFunc;
            SegmentSize = hasherFunc(Array.Empty<byte>()).Length;
            _span = new byte[SwarmChunk.SpanSize];
            
            // Define BMT depth and segment count.
            Depth = 1;
            SegmentCount = 2;
            while (SegmentCount < segmentCount)
            {
                Depth++;
                SegmentCount *= 2;
            }
            
            // Initialize lookup tables.
            zeroHashes = new byte[Depth + 1][];
            var zerosLayer = new byte[SegmentSize];
            zeroHashes[0] = zerosLayer;
            for (int i = 1; i < Depth + 1; i++)
            {
                zerosLayer = hasherFunc(zerosLayer.Concat(zerosLayer).ToArray());
                zeroHashes[i] = zerosLayer;
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
        /// The total length of the data (SegmentCount * SegmentSize)
        /// </summary>
        public int MaxSize => SegmentCount * SegmentSize;

        /// <summary>
        /// Index of rightmost currently open section
        /// </summary>
        public int SectionIndex
        {
            get
            {
                var index = WrittenSize / SectionSize;
                if (WrittenSize == MaxSize)
                    index--;
                return index;
            }
        }

        public int SectionSize => SegmentSize * 2;
        
        /// <summary>
        /// The number of segments on the base level of the BMT
        /// </summary>
        public int SegmentCount { get; }
        
        /// <summary>
        /// Size of leaf segments, stipulated to be = hash size
        /// </summary>
        public int SegmentSize { get; }

        /// <summary>
        /// The span of the data subsumed under the chunk
        /// </summary>
        public ReadOnlySpan<byte> Span
        {
            get => _span;
            set => _span = value.ToArray();
        }
        
        /// <summary>
        /// Bytes written to hasher since last Reset()
        /// </summary>
        public int WrittenSize { get; private set; }

        // Methods.
        public byte[] Hash()
        {
            if (WrittenSize == 0)
                return hasherFunc(_span.Concat(zeroHashes[Depth]).ToArray());

            Bmt.Buffer[WrittenSize..].Clear();
            
            // Hash the section.
            var offset = SectionIndex * SectionSize;
            var sectionHash = hasherFunc(Bmt.Buffer[offset..(offset + SectionSize)].ToArray());

            // Write hash into parent node and get result.
            var result =  WriteFinalNode(Bmt.Leaves[SectionIndex], sectionHash);
            
            return hasherFunc(_span.Concat(result).ToArray());
        }

        public void Reset()
        {
            WrittenSize = 0;
            Array.Fill(_span, (byte)0);
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            var maxWritable = MaxSize - WrittenSize;
            
            if (data.Length > maxWritable)
                throw new ArgumentOutOfRangeException(nameof(data), $"Max writable data is {maxWritable} bytes");
            
            var fromSection = SectionIndex;
            
            data.CopyTo(Bmt.Buffer[WrittenSize..]);
            WrittenSize += data.Length;
            
            // Process completed sections (except for the last one).
            for (var i = fromSection; i < SectionIndex; i++)
            {
                // Hash the section.
                var offset = i * SectionSize;
                var sectionHash = hasherFunc(Bmt.Buffer[offset..(offset + SectionSize)].ToArray());

                // Write hash into parent node.
                WriteNode(Bmt.Leaves[i], sectionHash);
            }
        }
        
        // Helpers.
        /// <summary>
        /// Pushes the data to the node.
        /// If it is the first of 2 sisters written, the routine terminates.
        /// If it is the second, it calculates the hash and writes it to the parent node recursively.
        /// Since hashing the parent is synchronous the same hasher can be used.
        /// </summary>
        private void WriteNode(BmtNode node, byte[] sectionHash)
        {
            var level = 1;
            var isLeft = node.IsLeft;
            var parentNode = node.Parent;
            
            while (true)
            {
                // At the root of the bmt just return the result.
                if (parentNode is null)
                    return;

                // Otherwise assign child hash to left or right segment.
                if (isLeft)
                    parentNode.Left = sectionHash;
                else
                    parentNode.Right = sectionHash;

                if (parentNode.Toggle())
                    return;
                
                // Calculates the hash of left|right and pushes it to the parent.
                sectionHash = hasherFunc(parentNode.Left!.Concat(parentNode.Right!).ToArray());
                isLeft = parentNode.IsLeft;
                parentNode = parentNode.Parent;
                level++;
            }
        }

        /// <summary>
        /// WriteFinalNode is following the path starting from the final datasegment to the BMT root via parents.
        /// For unbalanced trees it fills in the missing right sister nodes using the pool's lookup table for BMT subtree root hashes for all-zero sections.
        /// Otherwise behaves like `writeNode`.
        /// </summary>
        private byte[] WriteFinalNode(BmtNode node, byte[] sectionHash)
        {
            var level = 1;
            var isLeft = node.IsLeft;
            var parentNode = node.Parent;
            byte[]? currentHash = sectionHash;
            
            while (true)
            {
                // At the root of the bmt just return the result.
                if (parentNode is null)
                    return currentHash!;

                bool noHash;
                if (isLeft)
                {
                    // coming from left sister branch
                    // when the final section's path is going via left child node
                    // we include an all-zero subtree hash for the right level and toggle the node.
                    parentNode.Right = zeroHashes[level];
                    if(currentHash is not null)
                    {
                        parentNode.Left = currentHash;
                        
                        // If a left final node carries a hash, it must be the first (and only thread)
                        // so the toggle is already in passive state no need no call
                        // yet thread needs to carry on pushing hash to parent
                        noHash = false;
                    }
                    else
                    {
                        // if again first thread then propagate nil and calculate no hash
                        noHash = parentNode.Toggle();
                    }
                }
                else
                {
                    // right sister branch
                    if (currentHash is not null)
                    {
                        // if hash was pushed from right child node, write right segment change state
                        parentNode.Right = currentHash;
                        // if toggle is true, we arrived first so no hashing just push nil to parent
                        noHash = parentNode.Toggle();
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
                    currentHash = null;
                }
                else
                {
                    currentHash = hasherFunc(parentNode.Left!.Concat(parentNode.Right!).ToArray());
                }
                
                // iterate to parent
                isLeft = parentNode.IsLeft;
                parentNode = parentNode.Parent;
                level++;
            }
        }
    }
}