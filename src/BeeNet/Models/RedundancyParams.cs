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
using Etherna.BeeNet.Models.Bmt;
using Etherna.BeeNet.Services.Pipelines;
using STH1123.ReedSolomon;
using System;

namespace Etherna.BeeNet.Models
{
    public delegate void ParityChunkCallback(int level, byte[] span, byte[] address);
    
    public class RedundancyParams
    {
        // Fields.
        private PipelineStageBase pipeLine;
        /// <summary>
        /// keeps bytes of chunks on each level for producing erasure coded data; [levelIndex][branchIndex][byteIndex]
        /// </summary>
        private byte[][][] buffer;
        /// <summary>
        /// index of the current buffered chunk in Buffer. this is basically the latest used branchIndex.
        /// </summary>
        private int[] cursor;
        
        // Constructor.
        public RedundancyParams(RedundancyLevel level, bool encryption, PipelineStageBase pipeLine)
        {
            cursor = new int[9];
            Encryption = encryption;
            Level = level;
            this.pipeLine = pipeLine;
            MaxShards = 0;
            MaxParity = 0;
            
            if (encryption)
            {
                MaxShards = level.GetMaxEncShards();
                MaxParity = level.GetParities(SwarmBmt.EncryptedBranches);
            }
            else
            {
                MaxShards = level.GetMaxShards();
                MaxParity = level.GetParities(SwarmBmt.BmtBranches);
            }
            
            // init dataBuffer for erasure coding
            var rsChunkLevels = 0;
            
            if (level != RedundancyLevel.None)
                rsChunkLevels = 8;
            
            buffer = new byte[rsChunkLevels][][];
                
            for (var i = 0; i < rsChunkLevels; i++)
                buffer[i] = new byte[SwarmBmt.BmtBranches][]; // 128 long always because buffer varies at encrypted chunks
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
            Level.GetEncParities(shards):
            Level.GetParities(shards);

        /// <summary>
        /// ChunkWrite caches the chunk data on the given chunk level and if it is full then it calls Encode
        /// </summary>
        /// <param name="chunkLevel"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void ChunkWrite(int chunkLevel, byte[] data, ParityChunkCallback callback)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            if (Level == RedundancyLevel.None)
                return;

            if (data.Length != SwarmChunk.ChunkWithSpanSize)
                Array.Resize(ref data, SwarmChunk.ChunkWithSpanSize);

            // append chunk to the buffer
            buffer[chunkLevel][cursor[chunkLevel]] = data;
            cursor[chunkLevel]++;

            // add parity chunk if it is necessary
            if (cursor[chunkLevel] == MaxShards)
            {
                // append erasure coded data
                Encode(chunkLevel, callback);
            }
        }

        /// <summary>
        /// ElevateCarrierChunk moves the last poor orphan chunk to the level above where it can fit and there are other chunks as well.
        /// </summary>
        /// <param name="chunkLevel"></param>
        /// <param name="callback"></param>
        public void ElevateCarrierChunk(int chunkLevel, ParityChunkCallback callback)
        {
            if (Level == RedundancyLevel.None)
                return;

            if (cursor[chunkLevel] != 1)
                throw new InvalidOperationException(
                    $"redundancy: cannot elevate carrier chunk because it is not the only chunk on the level. It has {cursor[chunkLevel]} chunks");

            // not necessary to update current level since we will not work with it anymore
            ChunkWrite(chunkLevel + 1, buffer[chunkLevel][cursor[chunkLevel] - 1], callback);
        }

        /// <summary>
        /// Encode produces and stores parity chunks that will be also passed back to the caller
        /// </summary>
        /// <param name="chunkLevel"></param>
        /// <param name="callback"></param>
        public void Encode(int chunkLevel, ParityChunkCallback callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            
            var shards = cursor[chunkLevel];
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

                var args = new PipelineWriteContext
                {
                    Data = chunkData,
                    Span = span
                };
                pipeLine.ChainWrite(args);

                callback(chunkLevel + 1, span, args.Reference!);
            }
            cursor[chunkLevel] = 0;
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
            if (lastBuffer[0].Length != SwarmChunk.ChunkWithSpanSize)
                throw new InvalidOperationException(
                    "redundancy: hashtrie sum has not finished in order to cache root data");

            return lastBuffer[0];
        }
    }
}