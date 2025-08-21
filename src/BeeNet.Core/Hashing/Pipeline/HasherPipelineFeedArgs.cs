// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Models;
using System;
using System.Threading;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    public sealed class HasherPipelineFeedArgs
    {
        // Constructor.
        public HasherPipelineFeedArgs(
            SwarmChunkBmt swarmChunkBmt,
            ReadOnlyMemory<byte> span,
            ReadOnlyMemory<byte> spanData,
            long numberId = 0,
            SemaphoreSlim? prevChunkSemaphore = null)
        {
            if (span.Length != SwarmCac.SpanSize)
                throw new ArgumentOutOfRangeException(nameof(span), $"Span must have length of {SwarmCac.SpanSize}");
            if (spanData.Length < SwarmCac.SpanSize)
                throw new InvalidOperationException("Data must contain also span, if present");
            
            Span = span;
            SpanData = spanData;
            SwarmChunkBmt = swarmChunkBmt;
            NumberId = numberId;
            PrevChunkSemaphore = prevChunkSemaphore;
        }
        
        // Properties.
        public SwarmReference? ChunkReference { get; internal set; }

        /// <summary>
        /// Ordered id, from 0 to n with the last chunk
        /// </summary>
        public long NumberId { get; }

        /// <summary>
        /// Previous chunk semaphore. Occuped resource until chunk is processing.
        /// </summary>
        public SemaphoreSlim? PrevChunkSemaphore { get; }

        /// <summary>
        /// Always unecrypted span uint64
        /// </summary>
        public ReadOnlyMemory<byte> Span { get; }

        /// <summary>
        /// SpanData may be encrypted if the pipeline is encrypted
        /// </summary>
        public ReadOnlyMemory<byte> SpanData { get; internal set; }

        public SwarmChunkBmt SwarmChunkBmt { get; }
    }
}