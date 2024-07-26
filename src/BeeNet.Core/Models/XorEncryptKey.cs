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

using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Security.Cryptography;

namespace Etherna.BeeNet.Models
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
        
        public XorEncryptKey(string key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            
            try
            {
                bytes = key.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hash", nameof(key));
            }
            
            if (!IsValidKey(bytes))
                throw new ArgumentOutOfRangeException(nameof(key));
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
        
        public override string ToString() => bytes.ToHex();
        
        // Static methods.
        public static bool IsValidKey(byte[] value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            return value.Length == KeySize;
        }
    }
}