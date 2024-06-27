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
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Pipeline
{
    public interface IHasherPipeline : IDisposable
    {
        bool IsUsable { get; }
        
        /// <summary>
        /// Consume a byte array and returns a Swarm hash as result
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Resulting swarm hash</returns>
        Task<SwarmHash> HashDataAsync(byte[] data);

        /// <summary>
        /// Consume a stream slicing it in chunk size parts, and returns a Swarm hash as result
        /// </summary>
        /// <param name="dataStream">Input data stream</param>
        /// <returns>Resulting swarm hash</returns>
        Task<SwarmHash> HashDataAsync(Stream dataStream);
    }
}