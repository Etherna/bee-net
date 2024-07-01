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

namespace Etherna.BeeNet.Hasher.Postage
{
    public class PostageStamp
    {
        // Constructor.
        public PostageStamp(
            PostageBatchId batchId,
            StampBucketIndex stampBucketIndex,
            DateTimeOffset timeStamp,
            byte[] signature)
        {
            ArgumentNullException.ThrowIfNull(batchId, nameof(batchId));
            
            BatchId = batchId;
            StampBucketIndex = stampBucketIndex;
            TimeStamp = timeStamp;
            Signature = signature;
        }

        /// <summary>
        /// Postage batch ID
        /// </summary>
        public PostageBatchId BatchId { get; }

        /// <summary>
        /// Index of the batch
        /// </summary>
        public StampBucketIndex StampBucketIndex { get; }

        /// <summary>
        /// To signal order when assigning the indexes to multiple chunks
        /// </summary>
        public DateTimeOffset TimeStamp { get; }

        /// <summary>
        /// common r[32]s[32]v[1]-style 65 byte ECDSA signature of batchID|index|address by owner or grantee
        /// </summary>
        public ReadOnlyMemory<byte> Signature { get; }
    }
}