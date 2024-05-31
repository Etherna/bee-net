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
using Etherna.BeeNet.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Store
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class StampStoreItem : StoreItemBase
    {
        // Consts.
        public const int StampItemSize = SwarmAddress.HashSize * 2 + StampIndexSize + StampTimestampSize;
        
        // Constructor.
        public StampStoreItem(
            PostageBatchId batchId,
            SwarmAddress chunkAddress) :
            base(batchId, chunkAddress)
        { }

        // Properties.
        public override string Id =>
            string.Join("/",
                BatchId.ToString(),
                ChunkAddress.ToString());
        public byte[]? BatchIndex { get; set; }
        public DateTimeOffset? BatchTimestamp { get; set; }
        public override string NamespaceStr => "stampItem";
        
        // Methods.
        public override byte[] Marshal()
        {
            if (BatchIndex is null)
                throw new InvalidOperationException();
            if (BatchTimestamp is null)
                throw new InvalidOperationException();
            
            var buffer = new byte[StampItemSize + 1];

            var l = 0;
            BatchId.ToReadOnlySpan().CopyTo(buffer.AsSpan()[..SwarmAddress.HashSize]);
            l += SwarmAddress.HashSize;
            ChunkAddress.ToReadOnlySpan().CopyTo(buffer.AsSpan()[l..(l + SwarmAddress.HashSize)]);
            l += SwarmAddress.HashSize;
            Array.Copy(BatchIndex, 0, buffer, l, StampIndexSize);
            l += StampIndexSize;
            Array.Copy(BatchTimestamp.Value.ToUnixTimeMilliseconds().UnixDateTimeToByteArray(), 0, buffer, l, StampTimestampSize);

            return buffer;
        }

        public override void Unmarshal(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
            
            if (bytes.Length != StampItemSize + 1)
                throw new ArgumentOutOfRangeException(nameof(bytes));

            var l = 0;
            BatchId = new PostageBatchId(bytes[..SwarmAddress.HashSize]);
            l += SwarmAddress.HashSize;
            ChunkAddress = new SwarmAddress(bytes[l..(l + SwarmAddress.HashSize)]);
            l += SwarmAddress.HashSize;
            BatchIndex = bytes[l..(l + StampIndexSize)];
            l += StampIndexSize;
            BatchTimestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)bytes[l..(l + StampTimestampSize)].ByteArrayToUnixDateTime());
        }
    }
}