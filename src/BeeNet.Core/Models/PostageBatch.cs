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

using System;

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
            BzzBalance amount,
            ulong blockNumber,
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
            Label = label ?? "";
            Ttl = ttl;
            Utilization = utilization;
        }
        
        // Static properties.
        public static PostageBatch MaxDepthInstance { get; } = new(
            id: PostageBatchId.Zero,
            amount: 0,
            blockNumber: 0,
            depth: MaxDepth,
            exists: true,
            isImmutable: false,
            isUsable: true,
            label: null,
            ttl: TimeSpan.FromDays(3650),
            utilization: 0);

        // Properties.
        public PostageBatchId Id { get; }
        
        /// <summary>
        /// Amount paid for the batch
        /// </summary>
        public BzzBalance Amount { get; }
        
        /// <summary>
        /// Block number when this batch was created
        /// </summary>
        public ulong BlockNumber { get; }
        
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
        public string Label { get; }

        /// <summary>
        /// Time to live before expiration
        /// </summary>
        public TimeSpan Ttl { get; }
        
        /// <summary>
        /// The count of the fullest bucket
        /// </summary>
        public uint Utilization { get; }
        
        // Static methods.
        public static BzzBalance CalculateAmount(BzzBalance chainPrice, TimeSpan ttl) =>
            (decimal)(ttl / GnosisChain.BlockTime) * chainPrice;
        
        public static BzzBalance CalculatePrice(BzzBalance amount, int depth) =>
            amount * (decimal)Math.Pow(2, depth);

        public static BzzBalance CalculatePrice(BzzBalance chainPrice, TimeSpan ttl, int depth) =>
            CalculatePrice(CalculateAmount(chainPrice, ttl), depth);
    }
}
