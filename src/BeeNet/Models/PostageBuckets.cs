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
using System.Collections.Concurrent;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    /// <summary>
    /// A thread safe implementation of postage buckets array
    /// </summary>
    public class PostageBuckets
    {
        // Consts.
        public const int BucketsSize = 1 << PostageBatch.BucketDepth;
        
        // Fields.
        private readonly ConcurrentDictionary<uint, uint> _buckets; //<index, collisions>
        
        // Constructor.
        public PostageBuckets(
            uint[]? initialBuckets = null)
        {
            if (initialBuckets is not null &&
                initialBuckets.Length != BucketsSize)
                throw new ArgumentOutOfRangeException(nameof(initialBuckets),
                    $"Initial buckets must have length {BucketsSize}, or be null");
            
            _buckets = ArrayToDictionary(initialBuckets ?? []);
        }

        // Properties.
        public ReadOnlySpan<uint> Buckets => DictionaryToArray(_buckets);
        public uint MaxBucketCount { get; private set; }
        public long TotalChunks { get; private set; }
        
        // Methods.
        public uint GetCollisions(uint bucketId)
        {
            _buckets.TryGetValue(bucketId, out var collisions);
            return collisions;
        }

        public void IncrementCollisions(uint bucketId)
        {
            _buckets.AddOrUpdate(
                bucketId,
                _ =>
                {
                    TotalChunks++;
                    if (1 > MaxBucketCount)
                        MaxBucketCount = 1;
                    return 1;
                },
                (_, c) =>
                {
                    TotalChunks++;
                    if (c + 1 > MaxBucketCount)
                        MaxBucketCount = c + 1;
                    return c + 1;
                });
        }

        public void ResetBucketCollisions(uint bucketId) =>
            _buckets.AddOrUpdate(bucketId, _ => 0, (_, _) => 0);
        
        // Helpers.
        private static ConcurrentDictionary<uint, uint> ArrayToDictionary(uint[] buckets) =>
            new(buckets.Select((c, i) => (c, (uint)i))
                .ToDictionary<(uint value, uint index), uint, uint>(pair => pair.index, pair => pair.value));

        private static uint[] DictionaryToArray(ConcurrentDictionary<uint, uint> dictionary)
        {
            var outArray = new uint[BucketsSize];
            for (uint i = 0; i < BucketsSize; i++)
                if (dictionary.TryGetValue(i, out var value))
                    outArray[i] = value;
            return outArray;
        }
    }
}