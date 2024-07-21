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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Etherna.BeeNet.Models
{
    /// <summary>
    /// A thread safe implementation of postage buckets array
    /// </summary>
    [SuppressMessage("Reliability", "CA2002:Do not lock on objects with weak identity")]
    public class PostageBuckets : IReadOnlyPostageBuckets, IDisposable
    {
        // Consts.
        public const int BucketsSize = 1 << PostageBatch.BucketDepth;
        
        // Fields.
        private readonly uint[] _buckets; //number of collisions. MUST be private to permit locks on it
        private readonly Dictionary<uint, HashSet<uint>> bucketsByCollisions; //<collisions, bucketId[]>
        private readonly ReaderWriterLockSlim bucketsLock = new(LockRecursionPolicy.NoRecursion);
        private bool disposed;
        
        // Constructor.
        public PostageBuckets(
            uint[]? initialBuckets = null)
        {
            if (initialBuckets is not null &&
                initialBuckets.Length != BucketsSize)
                throw new ArgumentOutOfRangeException(nameof(initialBuckets),
                    $"Initial buckets must have length {BucketsSize}, or be null");
            
            //init "buckets" and reverse index "bucketsByCollisions"
            _buckets = initialBuckets ?? new uint[BucketsSize];
            bucketsByCollisions = new Dictionary<uint, HashSet<uint>> { [0] = [] };
            for (uint i = 0; i < BucketsSize; i++)
                bucketsByCollisions[0].Add(i);

            //init counters
            MaxBucketCollisions = 0;
            MinBucketCollisions = 0;
            TotalChunks = 0;
        }

        // Dispose.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            // Dispose managed resources.
            if (disposing)
                bucketsLock.Dispose();

            disposed = true;
        }

        // Properties.
        public uint MaxBucketCollisions { get; private set; }
        public uint MinBucketCollisions { get; private set; }
        public long TotalChunks { get; private set; }
        
        // Methods.
        public int[] CountBucketsByCollisions()
        {
            bucketsLock.EnterReadLock();
            try
            {
                return bucketsByCollisions.Select(pair => pair.Value.Count).ToArray();
            }
            finally
            {
                bucketsLock.ExitReadLock();
            }
        }
        
        public uint[] GetBuckets()
        {
            bucketsLock.EnterReadLock();
            try
            {
                return _buckets.ToArray();
            }
            finally
            {
                bucketsLock.ExitReadLock();
            }
        }

        public IEnumerable<uint> GetBucketsByCollisions(uint collisions)
        {
            bucketsLock.EnterReadLock();
            try
            {
                return bucketsByCollisions.TryGetValue(collisions, out var bucketsSet)
                    ? bucketsSet
                    : Array.Empty<uint>();
            }
            finally
            {
                bucketsLock.ExitReadLock();
            }
        }
        
        public uint GetCollisions(uint bucketId)
        {
            bucketsLock.EnterReadLock();
            try
            {
                return _buckets[bucketId];
            }
            finally
            {
                bucketsLock.ExitReadLock();
            }
        }

        public void IncrementCollisions(uint bucketId)
        {
            /*
             * We need to lock on the full _buckets because we need atomic operations also with bucketsByCollisions.
             * ConcurrentDictionary would have better locking on single values, but doesn't support atomic
             * operations involving third objects, like counters and "bucketsByCollisions".
             */
            bucketsLock.EnterWriteLock();
            try
            {
                // Update collections.
                _buckets[bucketId]++;
                
                bucketsByCollisions.TryAdd(_buckets[bucketId], []);
                bucketsByCollisions[_buckets[bucketId] - 1].Remove(bucketId);
                bucketsByCollisions[_buckets[bucketId]].Add(bucketId);
                
                // Update counters.
                if (_buckets[bucketId] > MaxBucketCollisions)
                    MaxBucketCollisions = _buckets[bucketId];
                    
                MinBucketCollisions = bucketsByCollisions.OrderBy(p => p.Key)
                    .First(p => p.Value.Count > 0)
                    .Key;
                    
                TotalChunks++;
            }
            finally
            {
                bucketsLock.ExitWriteLock();
            }
        }

        public void ResetBucketCollisions(uint bucketId)
        {
            bucketsLock.EnterWriteLock();
            try
            {
                // Update collections.
                var oldCollisions = _buckets[bucketId];
                _buckets[bucketId] = 0;
                
                bucketsByCollisions[oldCollisions].Remove(bucketId);
                bucketsByCollisions[0].Add(bucketId);
            
                // Update counters.
                MaxBucketCollisions = bucketsByCollisions.OrderByDescending(p => p.Key)
                    .First(p => p.Value.Count > 0)
                    .Key;
                
                MinBucketCollisions = 0;
            }
            finally
            {
                bucketsLock.ExitWriteLock();
            }
        }
    }
}