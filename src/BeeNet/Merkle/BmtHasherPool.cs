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

namespace Etherna.BeeNet.Merkle
{
    /// <summary>
    /// Implement a pool of Merkle Trees, defining a degree of parallelism to calculate hashes
    /// </summary>
    internal class BmtHasherPool : IDisposable
    {
        // Fields.
        private readonly BlockingCollection<BmtHasher> hasherCollection;

        // Constructor.
        public BmtHasherPool(int capacity, Func<byte[], byte[]> hasherFunc, int segmentCount)
        {
            Capacity = capacity;
            hasherCollection = new BlockingCollection<BmtHasher>(Capacity);
            for (var i = 0; i < Capacity; i++)
                hasherCollection.Add(new BmtHasher(hasherFunc, segmentCount));
        }

        // Dispose.
        public void Dispose()
        {
            hasherCollection.Dispose();
        }
        
        // Properties.
        /// <summary>
        /// Pool capacity, controls concurrency
        /// </summary>
        public int Capacity { get; }

        // Methods.
        public void Put(BmtHasher bmtHasher)
        {
            ArgumentNullException.ThrowIfNull(bmtHasher, nameof(bmtHasher));
            hasherCollection.Add(bmtHasher);
        }

        public BmtHasher Get() => hasherCollection.Take();
    }
}