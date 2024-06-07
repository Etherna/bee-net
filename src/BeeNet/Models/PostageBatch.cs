﻿//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public class PostageBatch
    {
        // Consts.
        public const int BucketDepth = 16;
        public const int MaxDepth = 47;
        public const int MinDepth = BucketDepth + 1;
        
        // Public constructor.
        public PostageBatch(
            PostageBatchId id,
            long amount,
            int blockNumber,
            int depth,
            bool exists,
            bool isImmutable,
            bool isUsable,
            string? label,
            TimeSpan ttl,
            uint utilization)
        {
            if (depth is < MinDepth or > MaxDepth)
                throw new ArgumentOutOfRangeException(nameof(depth), "Batch depth out of range");

            Id = id;
            Amount = amount;
            BlockNumber = blockNumber;
            Depth = depth;
            Exists = exists;
            IsImmutable = isImmutable;
            IsUsable = isUsable;
            Label = label;
            Ttl = ttl;
            Utilization = utilization;
        }
        
        // Internal constructors.
        internal PostageBatch(Clients.Stamps batch)
        {
            ArgumentNullException.ThrowIfNull(batch, nameof(batch));
            if (batch.Depth is < MinDepth or > MaxDepth)
                throw new ArgumentOutOfRangeException(nameof(batch), "Batch depth out of range");

            if (long.TryParse(batch.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                Amount = amount;
            Depth = batch.Depth;
            BlockNumber = batch.BlockNumber;
            Exists = batch.Exists;
            Id = batch.BatchID;
            IsImmutable = batch.ImmutableFlag;
            Label = batch.Label;
            Ttl = TimeSpan.FromSeconds(batch.BatchTTL);
            IsUsable = batch.Usable;
            Utilization = batch.Utilization;
        }

        internal PostageBatch(Clients.Response52 batch)
        {
            ArgumentNullException.ThrowIfNull(batch, nameof(batch));
            if (batch.Depth is < MinDepth or > MaxDepth)
                throw new ArgumentOutOfRangeException(nameof(batch), "Batch depth out of range");

            if (long.TryParse(batch.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                Amount = amount;
            Depth = batch.Depth;
            BlockNumber = batch.BlockNumber;
            Exists = batch.Exists;
            Id = batch.BatchID;
            IsImmutable = batch.ImmutableFlag;
            Label = batch.Label;
            Ttl = TimeSpan.FromSeconds(batch.BatchTTL);
            IsUsable = batch.Usable;
            Utilization = batch.Utilization;
        }
        
        // Static properties.
        public static PostageBatch MaxDepthInstance { get; } = new(
            PostageBatchId.Zero,
            0,
            0,
            MaxDepth,
            true,
            false,
            true,
            null,
            TimeSpan.FromDays(3650),
            0);

        // Properties.
        public PostageBatchId Id { get; }
        
        /// <summary>
        /// Amount paid for the batch
        /// </summary>
        public long Amount { get; }
        
        /// <summary>
        /// Block number when this batch was created
        /// </summary>
        public int BlockNumber { get; }
        
        /// <summary>
        /// Batch depth: batchSize = 2^depth * chunkSize
        /// </summary>
        public int Depth { get; }
        
        public bool Exists { get; }
        
        /// <summary>
        /// Specifies immutability of the batch
        /// </summary>
        public bool IsImmutable { get; }
        
        /// <summary>
        /// True if the batch is usable
        /// </summary>
        public bool IsUsable { get; }
        
        /// <summary>
        /// Label to identify the batch
        /// </summary>
        public string? Label { get; }
        
        /// <summary>
        /// Time to live before expiration
        /// </summary>
        public TimeSpan Ttl { get; }
        
        /// <summary>
        /// The count of the fullest bucket
        /// </summary>
        public uint Utilization { get; }
    }
}
