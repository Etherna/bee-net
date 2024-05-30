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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    internal class HashTriePipelineStage : PipelineStageBase
    {
        // Consts.
        private const int MaxLevel = 8;
        
        // Constructor.
        public HashTriePipelineStage(
            RedundancyParams redundancyParams,
            PipelineStageBase nextStage,
            IStoragePutter putter)
            : base(nextStage)
        {
            Cursors = new int[9];
            Buffer = new byte[SwarmChunk.SpanAndDataSize * 9 * 2]; // double size as temp workaround for weak calculation of needed buffer space
            RedundancyParams = redundancyParams ?? throw new ArgumentNullException(nameof(redundancyParams));
            ChunkCounters = new byte[9];
            EffectiveChunkCounters = new byte[9];
            MaxChildrenChunks = (byte)(redundancyParams.MaxShards + redundancyParams.Parities(redundancyParams.MaxShards));
            ReplicaPutter = new ReplicaPutter(putter, redundancyParams.Level);
            ParityChunkFn = (level, span, address) => WriteToIntermediateLevelAsync(level, true, span, address, Array.Empty<byte>());
        }

        // Properties.
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
        public bool IsFull { get; private set; }

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

        // Protected methods.
        protected override async Task FeedImplAsync(PipelineFeedArgs args)
        {
            var oneRef = SwarmAddress.HashSize + SwarmChunk.SpanSize;
            var l = args.Span.Length + SwarmAddress.HashSize + (args.EncryptionKey?.Length ?? 0);
            if (l%oneRef != 0 || l == 0)
            {
                throw new InvalidOperationException();
                //return errInconsistentRefs
            }

            if (IsFull)
            {
                throw new InvalidOperationException();
                //return errTrieFull
            }

            if (RedundancyParams.Level == RedundancyLevel.None)
            {
                await WriteToIntermediateLevelAsync(1, false, args.Span.ToArray(), args.Address!.Value, args.EncryptionKey).ConfigureAwait(false);
            } else
            {
                // write dataChunks to the level above
                await WriteToIntermediateLevelAsync(1, false, args.Span.ToArray(), args.Address!.Value, args.EncryptionKey).ConfigureAwait(false);

                await RedundancyParams.ChunkWriteAsync(0, args.Data.ToArray(), ParityChunkFn).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// returns the Swarm merkle-root content-addressed hash
        /// of an arbitrary-length binary data.
        /// The algorithm it uses is as follows:
        ///   - From level 1 till maxLevel 8, iterate:
        ///     -- If level data length equals 0 then continue to next level
        ///     -- If level data length equals 1 reference then carry over level data to next
        ///     -- If level data length is bigger than 1 reference then sum the level and
        ///     write the result to the next level
        ///   - Return the hash in level 8
        ///
        /// the cases are as follows:
        ///   - one hash in a given level, in which case we _do not_ perform a hashing operation, but just move
        ///     the hash to the next level, potentially resulting in a level wrap
        ///   - more than one hash, in which case we _do_ perform a hashing operation, appending the hash to
        ///     the next level
        /// </summary>
        /// <returns></returns>
        protected override async Task<byte[]> SumImplAsync()
        {
	        for (var i = 1; i < MaxLevel; i++)
            {
                var l = ChunkCounters[i];
                if (l == 0)
                {
                    // level empty, continue to the next.
                    continue;
                }
                if (l == MaxChildrenChunks)
                {
                    // this case is possible and necessary due to the carry over
                    // in the next switch case statement. normal writes done
                    // through writeToLevel will automatically wrap a full level.
                    // erasure encoding call is not necessary since ElevateCarrierChunk solves that
                    await WrapFullLevelAsync(i).ConfigureAwait(false);
                    continue;
                }
                if (l == 1)
                {
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
                    Cursors[i + 1] = Cursors[i];
                    // replace cached chunk to the level as well
                    await RedundancyParams.ElevateCarrierChunkAsync(i - 1, ParityChunkFn).ConfigureAwait(false);
                    // update counters, subtracting from current level is not necessary
                    EffectiveChunkCounters[i + 1]++;
                    ChunkCounters[i + 1]++;
                }
                else
                {
                    // call erasure encoding before writing the last chunk on the level
                    await RedundancyParams.EncodeAsync(i - 1, ParityChunkFn).ConfigureAwait(false);
                    // more than 0 but smaller than chunk size - wrap the level to the one above it
                    await WrapFullLevelAsync(i).ConfigureAwait(false);
                }
            }

            var levelLen = ChunkCounters[MaxLevel];
            if (levelLen != 1)
                throw new InvalidOperationException();

	        // return the hash in the highest level, that's all we need
            var data = Buffer[..Cursors[MaxLevel]];
            var rootHash = data[SwarmChunk.SpanSize..];

	        // save disperse replicas of the root chunk
	        if (RedundancyParams.Level != RedundancyLevel.None)
            {
                var rootData = RedundancyParams.GetRootData();
                ReplicaPutter.Put(new SwarmChunk(new SwarmAddress(rootHash[..SwarmAddress.HashSize]), rootData, true));
	        }

            return rootHash;
        }

        // Helpers.
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
        private async Task WrapFullLevelAsync(int level)
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
                var hash = data[offset..(offset + SwarmAddress.HashSize)];
                offset += SwarmAddress.HashSize;
                hashes = hashes.Concat(hash);
            }

            var parities = 0;
            while (offset < data.Length)
            {
                // we do not add span of parity chunks to the common because that is gibberish
                offset += SwarmChunk.SpanSize;
                var hash = data[offset..(offset + SwarmAddress.HashSize)]; // parity reference has always hash length
                offset += SwarmAddress.HashSize;
                hashes = hashes.Concat(hash);
                parities++;
            }

            var spb = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(spb, sp);
            if (parities > 0)
            {
                // EncodeLevel encodes used redundancy level for uploading into span keeping the real byte count for the chunk.
                // assumes span is LittleEndian.
                // set parity in the most signifact byte
                spb[SwarmChunk.SpanSize - 1] = (byte)((int)RedundancyParams.Level | (1 << 7)); // p + 128
            }
            hashes = spb.Concat(hashes);

            var args = new PipelineFeedArgs(
                data: hashes.ToArray(),
                span: spb);

            await FeedNextAsync(args).ConfigureAwait(false);
            
            await WriteToIntermediateLevelAsync(level + 1, false, args.Span.ToArray(), args.Address!.Value.ToByteArray(), args.EncryptionKey!).ConfigureAwait(false);
            await RedundancyParams.ChunkWriteAsync(level, args.Data.ToArray(), ParityChunkFn).ConfigureAwait(false);
            
            // this "truncates" the current level that was wrapped
            // by setting the cursors to the cursors of one level above
            Cursors[level] = Cursors[level + 1];
            ChunkCounters[level] = 0;
            EffectiveChunkCounters[level] = 0;

            if (level + 1 == MaxLevel)
                IsFull = true;
        }
        
        private async Task WriteToIntermediateLevelAsync(int level, bool parityChunk, byte[] span, SwarmAddress reference, byte[]? key)
        {
            ArgumentNullException.ThrowIfNull(span, nameof(span));
            ArgumentNullException.ThrowIfNull(reference, nameof(reference));

            Array.Copy(span, 0, Buffer, Cursors[level], span.Length);
            Cursors[level] += span.Length;
            Array.Copy(reference.ToByteArray(), 0, Buffer, Cursors[level], SwarmAddress.HashSize);
            Cursors[level] += SwarmAddress.HashSize;
            if (key != null)
            {
                Array.Copy(key, 0, Buffer, Cursors[level], key.Length);
                Cursors[level] += key.Length;
            }

            // update counters
            if (!parityChunk)
                EffectiveChunkCounters[level]++;
            ChunkCounters[level]++;
            if (ChunkCounters[level] == MaxChildrenChunks)
            {
                // at this point the erasure coded chunks have been written
                await WrapFullLevelAsync(level).ConfigureAwait(false);
            }
        }
    }
}