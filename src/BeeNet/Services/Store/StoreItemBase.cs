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
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Services.Store
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public abstract class StoreItemBase
    {
        public const int StampIndexSize = 8;
        public const int StampTimestampSize = 8;
        
        protected StoreItemBase(
            PostageBatchId batchId,
            SwarmAddress chunkAddress)
        {
            BatchId = batchId;
            ChunkAddress = chunkAddress;
        }

        /// <summary>
        /// The namespace of other related item
        /// </summary>
        public byte[] NamespaceByteArray { get; private set; } = default!;
        public PostageBatchId BatchId { get; protected set; }
        public byte[] StampIndex { get; private set; } = default!;
        public byte[] StampTimestamp { get; private set; } = default!;
        public SwarmAddress ChunkAddress { get; protected set; }
        public bool ChunkIsImmutable  { get; private set; }
        
        /// <summary>
        /// ID is the unique identifier of Item.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Namespace is used to separate similar items.
        /// E.g.: can be seen as a table construct.
        /// </summary>
        public abstract string NamespaceStr { get; }

        // Methods.
        public abstract byte[] Marshal();
        // public byte[] Marshal()
        // {
        //     if (NamespaceByteArray.Length == 0)
        //         throw new InvalidOperationException();
        //     if (StampIndex.Length != StampIndexSize)
        //         throw new InvalidOperationException();
        //
        //     var buf = new byte[
        //         8 + NamespaceByteArray.Length + PostageBatchId.BatchIdSize + StampIndexSize + StampTimestampSize + SwarmChunkBmt.SegmentSize + 1];
        //
        //     var l = 0;
        //     BinaryPrimitives.WriteUInt64LittleEndian(buf.AsSpan()[..8], (ulong)NamespaceByteArray.Length);
        //     l += 8;
        //     Array.Copy(NamespaceByteArray, buf, NamespaceByteArray.Length);
        //     l += NamespaceByteArray.Length;
        //     BatchId.ToReadOnlySpan().CopyTo(buf.AsSpan()[l..]);
        //     l += PostageBatchId.BatchIdSize;
        //     Array.Copy(StampIndex, 0, buf, l, StampIndexSize);
        //     l += StampIndexSize;
        //     Array.Copy(StampTimestamp, 0, buf, l, StampTimestampSize);
        //     l += StampTimestampSize;
        //     
        //     Array.Copy(ChunkAddress.ToByteArray(), 0, buf, l, SwarmChunkBmt.SegmentSize);
        //     l += SwarmChunkBmt.SegmentSize;
        //     buf[l] = (byte)(ChunkIsImmutable ? 1 : 0);
        //     return buf;
        // }

        public abstract void Unmarshal(byte[] bytes);
        // public void Unmarshal(byte[] bytes)
        // {
        //     ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
        //     
        //     if (bytes.Length < 8)
        //         throw new ArgumentOutOfRangeException(nameof(bytes));
        //     var nsLen = (int)BinaryPrimitives.ReadUInt64LittleEndian(bytes);
        //     if (bytes.Length != 8 + nsLen + SwarmChunkBmt.SegmentSize + StampIndexSize + StampTimestampSize +
        //         SwarmChunkBmt.SegmentSize + 1)
        //         throw new ArgumentOutOfRangeException(nameof(bytes));
        //
        //     var l = 8;
        //     NamespaceByteArray = bytes[l..(l + nsLen)];
        //     l += nsLen;
        //     BatchId = bytes[l..(l + SwarmChunkBmt.SegmentSize)];
        //     l += SwarmChunkBmt.SegmentSize;
        //     StampIndex = bytes[l..(l + StampIndexSize)];
        //     l += StampIndexSize;
        //     StampTimestamp = bytes[l..(l + StampTimestampSize)];
        //     l += StampTimestampSize;
        //     ChunkAddress = new SwarmAddress(bytes[l..(l + SwarmChunkBmt.SegmentSize)]);
        //     l += SwarmChunkBmt.SegmentSize;
        //     ChunkIsImmutable = bytes[l] == 1;
        // }
    }
}