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
    public class ObfuscationKey(
        byte[] bytes)
    {
        // Consts.
        public const int KeySize = 32;
        
        // Builders.
        public static ObfuscationKey BuildNewRandom()
        {
            var keyBytes = new byte[KeySize];
            RandomNumberGenerator.Fill(keyBytes);
            return new ObfuscationKey(keyBytes);
        }
        
        // Properties.
        public Memory<byte> Bytes => bytes;
        
        // Static properties.
        public static ObfuscationKey Empty { get; } = new(new byte[KeySize]);
        
        // Methods.
        /// <summary>
        /// Runs a XOR encryption on the input bytes, encrypting it if it
        /// hasn't already been, and decrypting it if it has, using the key provided.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted/decrypted data</returns>
        public byte[] EncryptDecrypt(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));
            
            var output = new byte[data.Length];

            for (var i = 0; i < data.Length; i++)
                output[i] = (byte)(data[i] ^ bytes[i % bytes.Length]);

            return output;
        }
    }
}