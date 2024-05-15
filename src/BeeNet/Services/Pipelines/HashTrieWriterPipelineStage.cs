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
using Etherna.BeeNet.Services.Putter;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Etherna.BeeNet.Services.Pipelines
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class HashTrieWriterPipelineStage : PipelineStageBase
    {
        // Constructor.
        public HashTrieWriterPipelineStage(
            int refLen,
            RedundancyParams redundancyParams,
            PipelineStageBase next,
            IPutter putter)
            : base(next)
        {
            RefSize = refLen;
            Cursors = new int[9];
            Buffer = new byte[SwarmChunk.ChunkWithSpanSize * 9 * 2]; // double size as temp workaround for weak calculation of needed buffer space
            RedundancyParams = redundancyParams ?? throw new ArgumentNullException(nameof(redundancyParams));
            ChunkCounters = new byte[9];
            EffectiveChunkCounters = new byte[9];
            MaxChildrenChunks =
                (byte)(redundancyParams.MaxShards + redundancyParams.Parities(redundancyParams.MaxShards));
            ReplicaPutter = new ReplicaPutter(putter, redundancyParams.Level);
            ParityChunkFn = (level, span, address) => WriteToIntermediateLevel(level, true, span, address, Array.Empty<byte>());
        }

        // Properties.
        public int RefSize { get; }

        /// <summary>
        /// Level cursors, key is level. level 0 is data level holds how many chunks were processed. Intermediate higher levels will always have LOWER cursor values.
        /// </summary>
        public int[] Cursors { get; }

        /// <summary>
        /// Keeps intermediate level data
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Indicates whether the trie is full. currently we support (128^7)*4096 = 2305843009213693952 bytes
        /// </summary>
        public bool Full { get; private set; }

        public RedundancyParams RedundancyParams { get; }

        public ParityChunkCallback ParityChunkFn { get; }

        /// <summary>
        /// Counts the chunk references in intermediate chunks. key is the chunk level.
        /// </summary>
        public byte[] ChunkCounters { get; }

        /// <summary>
        /// Counts the effective  chunk references in intermediate chunks. key is the chunk level.
        /// </summary>
        public byte[] EffectiveChunkCounters { get; }

        /// <summary>
        /// Maximum number of chunk references in intermediate chunks.
        /// </summary>
        public byte MaxChildrenChunks { get; }

        /// <summary>
        /// Putter to save dispersed replicas of the root chunk
        /// </summary>
        public ReplicaPutter ReplicaPutter { get; }
        
        // Methods.
        /// <summary>
        /// WrapLevel wraps an existing level and writes the resulting hash to the following level
        /// then truncates the current level data by shifting the cursors.
        /// Steps are performed in the following order:
        ///   - take all of the data in the current level
        ///   - break down span and hash data
        ///   - sum the span size, concatenate the hash to the buffer
        ///   - call the short pipeline with the span and the buffer
        ///   - get the hash that was created, append it one level above, and if necessary, wrap that level too
        ///   - remove already hashed data from buffer
        ///
        /// assumes that h.chunkCounters[level] has reached h.maxChildrenChunks at fullchunk
        /// or redundancy.Encode was called in case of rightmost chunks
        /// </summary>
        /// <param name="level"></param>
        public void WrapFullLevel(int level)
        {
            var data = Buffer[Cursors[level + 1]..Cursors[level]];
            ulong sp = 0;
            IEnumerable<byte> hashes = Array.Empty<byte>();
            var offset = 0;

            for (int i = 0; i < EffectiveChunkCounters[level]; i++)
            {
                // sum up the spans of the level, then we need to bmt them and store it as a chunk
                // then write the chunk address to the next level up
                sp += BitConverter.ToUInt64(data[offset..(offset + SwarmChunk.SpanSize)], 0);
                offset += SwarmChunk.SpanSize;
                var hash = data[offset..(offset + RefSize)];
                offset += RefSize;
                hashes = hashes.Concat(hash);
            }

            var parities = 0;
            while (offset < data.Length)
            {
                // we do not add span of parity chunks to the common because that is gibberish
                offset += SwarmChunk.SpanSize;
                var hash = data[offset..(offset + SwarmAddress.HashByteSize)]; // parity reference has always hash length
                offset += SwarmAddress.HashByteSize;
                hashes = hashes.Concat(hash);
                parities++;
            }

            var spb = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(spb, sp);
            if (parities > 0)
                EncodeLevel(spb, RedundancyParams.Level);
            hashes = spb.Concat(hashes);

            var args = new PipelineWriteContext
            {
                Data = hashes.ToArray(),
                Span = spb
            };
            Next!.ChainWrite(args);
            
            WriteToIntermediateLevel(level + 1, false, args.Span, args.Reference!, args.EncryptionKey!);
            RedundancyParams.ChunkWrite(level, args.Data, ParityChunkFn);
            
            // this "truncates" the current level that was wrapped
            // by setting the cursors to the cursors of one level above
            Cursors[level] = Cursors[level + 1];
            ChunkCounters[level] = 0;
            EffectiveChunkCounters[level] = 0;

            if (level + 1 == 8)
                Full = true;
        }
        
        public void WriteToIntermediateLevel(int level, bool parityChunk, byte[] span, byte[] reference, byte[] key)
        {
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            ArgumentNullException.ThrowIfNull(reference, nameof(reference));
            ArgumentNullException.ThrowIfNull(key, nameof(key));

            Array.Copy(span, 0, Buffer, Cursors[level], span.Length);
            Cursors[level] += span.Length;
            Array.Copy(reference, 0, Buffer, Cursors[level], reference.Length);
            Cursors[level] += reference.Length;
            Array.Copy(key, 0, Buffer, Cursors[level], key.Length);
            Cursors[level] += key.Length;

            // update counters
            if (!parityChunk)
                EffectiveChunkCounters[level]++;
            ChunkCounters[level]++;
            if (ChunkCounters[level] == MaxChildrenChunks)
            {
                // at this point the erasure coded chunks have been written
                WrapFullLevel(level);
            }
        }

        public override byte[] Sum()
        {
	        for i := 1; i < maxLevel; i++ {
		        l := h.chunkCounters[i]
		        switch {
		        case l == 0:
			        // level empty, continue to the next.
			        continue
		        case l == h.maxChildrenChunks:
			        // this case is possible and necessary due to the carry over
			        // in the next switch case statement. normal writes done
			        // through writeToLevel will automatically wrap a full level.
			        // erasure encoding call is not necessary since ElevateCarrierChunk solves that
			        err := h.wrapFullLevel(i)
			        if err != nil {
				        return nil, err
			        }
		        case l == 1:
			        // this cursor assignment basically means:
			        // take the hash|span|key from this level, and append it to
			        // the data of the next level. you may wonder how this works:
			        // every time we sum a level, the sum gets written into the next level
			        // and the level cursor gets set to the next level's cursor (see the
			        // truncating at the end of wrapFullLevel). there might (or not) be
			        // a hash at the next level, and the cursor of the next level is
			        // necessarily _smaller_ than the cursor of this level, so in fact what
			        // happens is that due to the shifting of the cursors, the data of this
			        // level will appear to be concatenated with the data of the next level.
			        // we therefore get a "carry-over" behavior between intermediate levels
			        // that might or might not have data. the eventual result is that the last
			        // hash generated will always be carried over to the last level (8), then returned.
			        h.cursors[i+1] = h.cursors[i]
			        // replace cached chunk to the level as well
			        err := h.rParams.ElevateCarrierChunk(i-1, h.parityChunkFn)
			        if err != nil {
				        return nil, err
			        }
			        // update counters, subtracting from current level is not necessary
			        h.effectiveChunkCounters[i+1]++
			        h.chunkCounters[i+1]++
		        default:
			        // call erasure encoding before writing the last chunk on the level
			        err := h.rParams.Encode(i-1, h.parityChunkFn)
			        if err != nil {
				        return nil, err
			        }
			        // more than 0 but smaller than chunk size - wrap the level to the one above it
			        err = h.wrapFullLevel(i)
			        if err != nil {
				        return nil, err
			        }
		        }
	        }
	        levelLen := h.chunkCounters[maxLevel]
	        if levelLen != 1 {
		        return nil, errInconsistentRefs
	        }

	        // return the hash in the highest level, that's all we need
	        data := h.buffer[0:h.cursors[maxLevel]]
	        rootHash := data[swarm.SpanSize:]

	        // save disperse replicas of the root chunk
	        if h.rParams.Level() != redundancy.NONE {
		        rootData, err := h.rParams.GetRootData()
		        if err != nil {
			        return nil, err
		        }
		        err = h.replicaPutter.Put(h.ctx, swarm.NewChunk(swarm.NewAddress(rootHash[:swarm.HashSize]), rootData))
		        if err != nil {
			        return nil, fmt.Errorf("hashtrie: cannot put dispersed replica %s", err.Error())
		        }
	        }
	        return rootHash, nil
        }

        // Helpers.
        /// <summary>
        /// EncodeLevel encodes used redundancy level for uploading into span keeping the real byte count for the chunk.
        /// assumes span is LittleEndian
        /// </summary>
        /// <param name="span"></param>
        /// <param name="level"></param>
        private static void EncodeLevel(byte[] span, RedundancyLevel level)
        {
            // set parity in the most signifact byte
            span[SwarmChunk.SpanSize - 1] = (byte)((int)level | (1 << 7)); // p + 128
        }
    }
}