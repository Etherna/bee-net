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
using System.Threading;

namespace Etherna.BeeNet.Models
{
    /// <summary>
    /// Implement a pool of Merkle Trees, defining a degree of parallelism to calculate hashes
    /// </summary>
    internal class SwarmChunkBmtHasherPool : IDisposable
    {
        // Fields.
        private readonly SemaphoreSlim semaphore;

        // Constructor.
        private SwarmChunkBmtHasherPool(int capacity)
        {
            semaphore = new SemaphoreSlim(capacity, capacity);
        }

        // Dispose.
        public void Dispose()
        {
            semaphore.Dispose();
        }
        
        // Static properties.
        public static SwarmChunkBmtHasherPool Instance { get; } = new(
            Environment.ProcessorCount * 4);

        // Methods.
        public byte[] Hash(ReadOnlySpan<byte> span, ReadOnlySpan<byte> data)
        {
            semaphore.Wait();
            
            try
            {
                return SwarmChunkBmtHasher.Hash(span, data);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}