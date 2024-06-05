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
using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Pipeline
{
    public interface IHasherPipeline : IDisposable
    {
        bool IsUsable { get; }
        
        /// <summary>
        /// Consume a byte array and returns a Swarm address as result
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Resulting swarm address</returns>
        Task<SwarmAddress> HashDataAsync(byte[] data);

        /// <summary>
        /// Consume a stream slicing it in chunk size parts, and returns a Swarm address as result
        /// </summary>
        /// <param name="dataStream">Input data stream</param>
        /// <returns>Resulting swarm address</returns>
        Task<SwarmAddress> HashDataAsync(Stream dataStream);
    }
}