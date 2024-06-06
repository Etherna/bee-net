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
using System.Security.Cryptography;

namespace Etherna.BeeNet.Manifest
{
    public class XorEncryptKey
    {
        // Consts.
        public const int KeySize = 32;
        
        // Fields.
        private readonly byte[] bytes;

        // Constructor.
        public XorEncryptKey(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
            if (bytes.Length != KeySize)
                throw new ArgumentOutOfRangeException(nameof(bytes));
            
            this.bytes = bytes;
        }

        // Builders.
        public static XorEncryptKey BuildNewRandom()
        {
            var keyBytes = new byte[KeySize];
            RandomNumberGenerator.Fill(keyBytes);
            return new XorEncryptKey(keyBytes);
        }
        
        // Properties.
        public Memory<byte> Bytes => bytes;
        
        // Static properties.
        public static XorEncryptKey Empty { get; } = new(new byte[KeySize]);
        
        // Methods.
        /// <summary>
        /// Runs a XOR encryption on the input bytes, encrypting it if it
        /// hasn't already been, and decrypting it if it has, using the key provided.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted/decrypted data</returns>
        public void EncryptDecrypt(Span<byte> data)
        {
            for (var i = 0; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ bytes[i % bytes.Length]);
        }
    }
}