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
using Etherna.BeeNet.Postage;
using Etherna.BeeNet.Redundancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Pipelines
{
    internal class ChunkAggregatorPipelineStage : PipelineStageBase
    {
        // Classes.
        private class ChunkInfo(bool isParityChunk, ReadOnlyMemory<byte> span, SwarmAddress address, byte[]? key)
        {
            public bool IsParityChunk { get; } = isParityChunk;
            public ReadOnlyMemory<byte> Span { get; } = span;
            public SwarmAddress Address { get; } = address;
            public byte[] Key { get; } = key ?? [];

            public IEnumerable<byte> ToByteEnumerable() =>
                Span.ToArray().Concat(
                Address.ToByteArray()).Concat(
                Key);
        }

        private class ChunkInfoLevel
        {
            private readonly List<ChunkInfo> _chunks = new();

            public IReadOnlyList<ChunkInfo> Chunks => _chunks.AsReadOnly();
            public int CountChunks => _chunks.Count;
            public int ParityChunksCounter => _chunks.Count(c => c.IsParityChunk);

            public void AddChunk(ChunkInfo chunkInfo) => _chunks.Add(chunkInfo);
            public void ClearChunks() => _chunks.Clear();
            public ChunkInfo GetChunk(int i) => _chunks[i];
            
            public IEnumerable<byte> ToByteEnumerable()
            {
                IEnumerable<byte> enumerable = Array.Empty<byte>();
                foreach (var chunk in _chunks)
                    enumerable = enumerable.Concat(chunk.ToByteEnumerable());
                return enumerable;
            }
        }
        
        // Consts.
        private const int MaxLevel = 8;  //could be removed after refactoring
        
        // Fields.
        private readonly AddParityChunkCallback addParityChunkFn;
        private readonly List<ChunkInfoLevel> chunkLevels;
        private readonly RedundancyParams redundancyParams;
        private readonly PipelineStageBase hashingPipeline;
        
        // Constructor.
        public ChunkAggregatorPipelineStage(
            RedundancyParams redundancyParams,
            PipelineStageBase hashingPipeline,
            IPostageStamper postageStamper)
            : base(null)
        {
            chunkLevels = [];
            for (int i = 0; i < MaxLevel + 1; i++)
                chunkLevels.Add(new ChunkInfoLevel());
            this.hashingPipeline = hashingPipeline ?? throw new ArgumentNullException(nameof(hashingPipeline));
            
            this.redundancyParams = redundancyParams ?? throw new ArgumentNullException(nameof(redundancyParams));
            MaxChildrenChunks = (byte)(redundancyParams.MaxShards + redundancyParams.Parities(redundancyParams.MaxShards));
            ReplicaPutter = new ReplicaPutter(postageStamper, redundancyParams.Level);
            addParityChunkFn = (level, span, address) =>
                AddChunkToLevelAsync(level, new ChunkInfo(true, span, address, null));
        }

        // Properties.
        /// <summary>
        /// Indicates whether the collector is full. currently we support (128^7)*4096 = 2305843009213693952 bytes
        /// </summary>
        public bool IsFull { get; private set; }

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
            if (IsFull)
                throw new InvalidOperationException();

            await AddChunkToLevelAsync(
                1,
                new ChunkInfo(false, args.Span, args.Address!.Value, args.EncryptionKey)).ConfigureAwait(false);
            
            if (redundancyParams.Level != RedundancyLevel.None)
                await redundancyParams.ChunkWriteAsync(
                    0,
                    args.Data.ToArray(),
                    addParityChunkFn).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the Swarm merkle-root content-addressed hash of an arbitrary-length binary data.
        /// </summary>
        /// <returns>The Swarm merkle-root content-addressed hash</returns>
        protected override async Task<SwarmAddress> SumImplAsync()
        {
            // The algorithm it uses is as follows:
            //   - From level 1 till maxLevel 8, iterate:
            //     -- If level data length equals 0 then continue to next level
            //     -- If level data length equals 1 reference then carry over level data to next
            //     -- If level data length is bigger than 1 reference then sum the level and write the result to the next level
            //   - Return the hash in level 8
            //
            // the cases are as follows:
            //   - one hash in a given level, in which case we _do not_ perform a hashing operation, but just move
            //     the hash to the next level, potentially resulting in a level wrap
            //   - more than one hash, in which case we _do_ perform a hashing operation, appending the hash to the next level
            for (var i = 1; i < chunkLevels.Count - 1; i++)
            {
                switch (chunkLevels[i].CountChunks)
                {
                    case 0: break; //level empty, continue to the next
                    case 1:
                        chunkLevels[i + 1].ClearChunks(); //probably not necessary
                        chunkLevels[i + 1].AddChunk(chunkLevels[i].GetChunk(0));

                        //replace cached chunk to the level as well
                        await redundancyParams.ElevateCarrierChunkAsync(i - 1, addParityChunkFn).ConfigureAwait(false);
                        break;
                    default:
                        if (chunkLevels[i].CountChunks != MaxChildrenChunks)
                        {
                            // call erasure encoding before writing the last chunk on the level
                            await redundancyParams.EncodeAsync(i - 1, addParityChunkFn).ConfigureAwait(false);
                        }

                        await WrapFullLevelAsync(i).ConfigureAwait(false);
                        break;
                }
            }

            var lastLevel = chunkLevels.Last();
            if (lastLevel.CountChunks != 1)
                throw new InvalidOperationException();
            
            var rootChunk = lastLevel.GetChunk(0);

	        // save disperse replicas of the root chunk
	        if (redundancyParams.Level != RedundancyLevel.None)
            {
                var rootData = redundancyParams.GetRootData();
                ReplicaPutter.Put(SwarmChunk.BuildFromSpanAndData(rootChunk.Address, rootData));
	        }

            return rootChunk.Address;
        }

        // Helpers.
        private async Task AddChunkToLevelAsync(int level, ChunkInfo chunkInfo)
        {
            ArgumentNullException.ThrowIfNull(chunkInfo, nameof(chunkInfo));
            
            chunkLevels[level].AddChunk(chunkInfo);
            if (chunkLevels[level].CountChunks == MaxChildrenChunks)
            {
                //at this point the erasure coded chunks have been written
                await WrapFullLevelAsync(level).ConfigureAwait(false);
            }
        }
        
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
            var chunkLevel = chunkLevels[level];

            // Sum up the spans of the level, then we need to bmt them and store it as a chunk
            // then write the chunk address to the next level up.
            // We do not add span of parity chunks to the common because that is gibberish.
            var totalSpan = SwarmChunk.LengthToSpan(
                chunkLevel.Chunks.Where(c => !c.IsParityChunk)
                    .Select(c => SwarmChunk.SpanToLength(c.Span.Span))
                    .Aggregate((a,c) => a + c)); //sum of ulongs. Linq doesn't have it
            
            if (chunkLevel.ParityChunksCounter > 0)
            {
                // EncodeLevel encodes used redundancy level for uploading into span keeping the real byte count for the chunk.
                // assumes span is LittleEndian.
                // set parity in the most signifact byte
                totalSpan[SwarmChunk.SpanSize - 1] = (byte)((int)redundancyParams.Level | (1 << 7)); // p + 128
            }
            
            // Build total data from total span, and all the addresses in level.
            var totalData = totalSpan.Concat(
                chunkLevel.Chunks.SelectMany(c => c.Address.ToByteArray()));

            // Run Bmt on the new chunk, using the short hashing pipeline.
            var pipelineArgs = new PipelineFeedArgs(
                data: totalData.ToArray(),
                span: totalSpan);

            await hashingPipeline.FeedAsync(pipelineArgs).ConfigureAwait(false);
            
            await AddChunkToLevelAsync(
                level + 1,
                new ChunkInfo(
                    false,
                    pipelineArgs.Span,
                    pipelineArgs.Address!.Value,
                    pipelineArgs.EncryptionKey)).ConfigureAwait(false);
            
            await redundancyParams.ChunkWriteAsync(level, pipelineArgs.Data.ToArray(), addParityChunkFn).ConfigureAwait(false);
            
            chunkLevel.ClearChunks();

            if (level + 1 == MaxLevel)
                IsFull = true;
        }
    }
}