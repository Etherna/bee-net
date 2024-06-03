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

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hasher.Bmt;
using Etherna.BeeNet.Hasher.Pipeline;
using Etherna.BeeNet.Models;
using STH1123.ReedSolomon;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Redundancy
{
    internal delegate Task AddParityChunkCallback(int level, byte[] span, byte[] address);
    
    internal class RedundancyParams
    {
        // Consts.
        public const int EncryptedBmtSegments = SwarmChunkBmt.SegmentsCount / 2;
        
        // Fields.
        private readonly IHasherPipelineStage pipeLine;
        
        /// <summary>
        /// Keeps bytes of chunks on each level for producing erasure coded data: [levelIndex][branchIndex][byteIndex]
        /// </summary>
        private readonly byte[][][] buffer;
        
        /// <summary>
        /// Index of the current buffered chunk in Buffer. this is basically the latest used branchIndex.
        /// </summary>
        private readonly int[] bufferCursor;
        
        // Constructor.
        public RedundancyParams(
            RedundancyLevel level,
            bool encryption,
            IHasherPipelineStage pipeLine)
        {
            bufferCursor = new int[9];
            Encryption = encryption;
            MaxParity = level.GetParities(encryption ? EncryptedBmtSegments : SwarmChunkBmt.SegmentsCount);
            MaxShards = encryption ? level.GetMaxEncryptedShards() : level.GetMaxShards();
            Level = level;
            this.pipeLine = pipeLine;

            // Init data buffer for erasure coding.
            buffer = new byte[level == RedundancyLevel.None ? 0 : 8][][];
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = new byte[SwarmChunkBmt.SegmentsCount][]; // 128 long always because buffer varies at encrypted chunks
        }
        
        // Properties.
        public bool Encryption { get; }
        
        /// <summary>
        /// number of parity chunks if maxShards has been reached for erasure coding
        /// </summary>
        public int MaxParity { get; }
        
        /// <summary>
        /// Number of chunks after which the parity encode function should be called
        /// </summary>
        public int MaxShards { get; }

        public RedundancyLevel Level { get; }

        // Methods.
        public int Parities(int shards) => Encryption ?
            Level.GetEncryptedParities(shards):
            Level.GetParities(shards);

        /// <summary>
        /// ChunkWrite caches the chunk data on the given chunk level and if it is full then it calls Encode
        /// </summary>
        /// <param name="chunkLevel"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public async Task ChunkWriteAsync(int chunkLevel, byte[] data, AddParityChunkCallback callback)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            if (Level == RedundancyLevel.None)
                return;

            if (data.Length != SwarmChunk.SpanAndDataSize)
                Array.Resize(ref data, SwarmChunk.SpanAndDataSize);

            // append chunk to the buffer
            buffer[chunkLevel][bufferCursor[chunkLevel]] = data;
            bufferCursor[chunkLevel]++;

            // add parity chunk if it is necessary
            if (bufferCursor[chunkLevel] == MaxShards)
            {
                // append erasure coded data
                await EncodeAsync(chunkLevel, callback).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// ElevateCarrierChunk moves the last poor orphan chunk to the level above where it can fit and there are other chunks as well.
        /// </summary>
        /// <param name="chunkLevel"></param>
        /// <param name="callback"></param>
        public async Task ElevateCarrierChunkAsync(int chunkLevel, AddParityChunkCallback callback)
        {
            if (Level == RedundancyLevel.None)
                return;

            if (bufferCursor[chunkLevel] != 1)
                throw new InvalidOperationException(
                    $"redundancy: cannot elevate carrier chunk because it is not the only chunk on the level. It has {bufferCursor[chunkLevel]} chunks");

            // not necessary to update current level since we will not work with it anymore
            await ChunkWriteAsync(chunkLevel + 1, buffer[chunkLevel][bufferCursor[chunkLevel] - 1], callback).ConfigureAwait(false);
        }

        /// <summary>
        /// Encode produces and stores parity chunks that will be also passed back to the caller
        /// </summary>
        /// <param name="chunkLevel"></param>
        /// <param name="callback"></param>
        public async Task EncodeAsync(int chunkLevel, AddParityChunkCallback callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            if (Level == RedundancyLevel.None || bufferCursor[chunkLevel] == 0)
                return;
            
            var shards = bufferCursor[chunkLevel];
            var parities = Parities(shards);

            var n = shards + parities;
            
            // realloc for parity chunks if it does not override the prev one
            // calculate parity chunks
            var enc = new ReedSolomonEncoder(new GenericGF(0, shards, 0)); //**** <- FIX THIS. Original: erasureEncoderFunc(shards, parities);

            var pz = buffer[chunkLevel][0].Length;
            for (var i = shards; i < n; i++)
                buffer[chunkLevel][i] = new byte[pz];
            
            enc.Encode(Array.Empty<int>() /*buffer[chunkLevel][..n]*/, 0);  //**** <- FIX THIS

            for (var i = shards; i < n; i++)
            {
                var chunkData = buffer[chunkLevel][i];
                var span = chunkData[..SwarmChunk.SpanSize];

                var args = new HasherPipelineFeedArgs(
                    data: chunkData,
                    span: span);
                await pipeLine.FeedAsync(args).ConfigureAwait(false);

                await callback(chunkLevel + 1, span, args.Address!.Value.ToByteArray()).ConfigureAwait(false);
            }
            bufferCursor[chunkLevel] = 0;
        }

        /// <summary>
        /// GetRootData returns the topmost chunk in the tree.
        /// throws and error if the encoding has not been finished in the BMT
        /// OR redundancy is not used in the BMT
        /// </summary>
        /// <returns></returns>
        public byte[] GetRootData()
        {
            if (Level == RedundancyLevel.None)
                throw new InvalidOperationException("redundancy: no redundancy level is used for the file in order to cache root data");
            
            var lastBuffer = buffer[^1];
            if (lastBuffer[0].Length != SwarmChunk.SpanAndDataSize)
                throw new InvalidOperationException(
                    "redundancy: hashtrie sum has not finished in order to cache root data");

            return lastBuffer[0];
        }
    }
}