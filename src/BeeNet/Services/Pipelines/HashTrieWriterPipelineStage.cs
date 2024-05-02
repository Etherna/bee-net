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
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Services.Pipelines
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class HashTrieWriterPipelineStage : PipelineStageBase
    {
        public HashTrieWriterPipelineStage(PipelineStageBase? next)
            : base(next)
        {
        }

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
        public bool full { get; }

        public RedundancyParams RedundancyParams { get; }

        public RedundancyParityChunkCallback ParityChunkFn { get; }

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
        public StamperPutter ReplicaPutter { get; }
    }
}