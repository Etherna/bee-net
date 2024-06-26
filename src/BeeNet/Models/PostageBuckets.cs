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