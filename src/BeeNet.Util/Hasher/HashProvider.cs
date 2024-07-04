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
using Nethereum.Util.HashProviders;
using Org.BouncyCastle.Crypto.Digests;
using System;

namespace Etherna.BeeNet.Hasher
{
    public class HashProvider : IHashProvider
    {
        // Fields.
        private readonly KeccakDigest hasher = new(256);
        
        // Methods.
        public void ComputeHash(byte[] data, Span<byte> output)
        {
            hasher.BlockUpdate(data);
            hasher.DoFinal(output);
        }
        
        public byte[] ComputeHash(byte[] data)
        {
            var result = new byte[SwarmHash.HashSize];
            ComputeHash(data, result);
            return result;
        }

        public byte[] ComputeHash(string data) =>
            ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
    }
}