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
        public void ComputeHash(byte[] data, Span<byte> output)
        {
            var hash = ComputeHash(data);
            hash.CopyTo(output);

            /*
             * With BouncyCastle >= 2.0.0. Downgrade required by Nethereum.
             * See: https://github.com/Nethereum/Nethereum/releases/tag/4.27.1
             */
            // hasher.BlockUpdate(data);
            // hasher.DoFinal(output);
        }
        
        public byte[] ComputeHash(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            var output = new byte[hasher.GetDigestSize()];
            hasher.BlockUpdate(data, 0, data.Length);
            hasher.DoFinal(output, 0);
            return output;
            
            /*
             * With BouncyCastle >= 2.0.0. Downgrade required by Nethereum.
             * See: https://github.com/Nethereum/Nethereum/releases/tag/4.27.1
             */
            // var result = new byte[SwarmHash.HashSize];
            // ComputeHash(data, result);
            // return result;
        }
        
        public byte[] ComputeHash(params byte[][] dataArray)
        {
            ArgumentNullException.ThrowIfNull(dataArray, nameof(dataArray));
            
            var output = new byte[hasher.GetDigestSize()];
            foreach (var data in dataArray)
                hasher.BlockUpdate(data, 0, data.Length);
            hasher.DoFinal(output, 0);
            return output;
        }

        public byte[] ComputeHash(string data) =>
            ComputeHash(Encoding.UTF8.GetBytes(data));
    }
}