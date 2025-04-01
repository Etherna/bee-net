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
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Text;

namespace Etherna.BeeNet.Hashing
{
    /// <summary>
    /// Hasher service, not thread safe
    /// </summary>
    public class Hasher : IHasher
    {
        // Fields.
        private readonly KeccakDigest hasher = new(256);
        
        // Methods.
        public byte[] ComputeHash(string data) => ComputeHash(Encoding.UTF8.GetBytes(data));
        public byte[] ComputeHash(ReadOnlySpan<byte> data)
        {
            var result = new byte[SwarmHash.HashSize];
            ComputeHash(data, result);
            return result;
        }
        public byte[] ComputeHash(ReadOnlyMemory<byte>[] dataArray)
        {
            var result = new byte[SwarmHash.HashSize];
            ComputeHash(dataArray, result);
            return result;
        }
        public void ComputeHash(string data, Span<byte> output) =>
            ComputeHash(Encoding.UTF8.GetBytes(data), output);
        public void ComputeHash(ReadOnlySpan<byte> data, Span<byte> output)
        {
            hasher.BlockUpdate(data);
            hasher.DoFinal(output);
        }
        public void ComputeHash(ReadOnlyMemory<byte>[] dataArray, Span<byte> output)
        {
            ArgumentNullException.ThrowIfNull(dataArray, nameof(dataArray));
            ArgumentOutOfRangeException.ThrowIfNotEqual(output.Length, SwarmHash.HashSize);
            
            foreach (var data in dataArray)
                hasher.BlockUpdate(data.Span);
            hasher.DoFinal(output);
        }
    }
}