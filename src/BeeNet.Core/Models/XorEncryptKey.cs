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

using Etherna.BeeNet.TypeConverters;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(XorEncryptKeyTypeConverter))]
    public readonly struct XorEncryptKey : IEquatable<XorEncryptKey>
    {
        // Consts.
        public const int KeySize = 32;
        
        // Fields.
        private readonly byte[] byteKey;

        // Constructor.
        public XorEncryptKey(byte[] key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            if (!IsValidKey(key))
                throw new ArgumentOutOfRangeException(nameof(key));
            
            byteKey = key;
        }
        
        public XorEncryptKey(string key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            
            try
            {
                byteKey = key.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hash", nameof(key));
            }
            
            if (!IsValidKey(byteKey))
                throw new ArgumentOutOfRangeException(nameof(key));
        }

        // Builders.
        public static XorEncryptKey BuildNewRandom()
        {
            var keyBytes = new byte[KeySize];
            RandomNumberGenerator.Fill(keyBytes);
            return new XorEncryptKey(keyBytes);
        }
        
        // Static properties.
        public static XorEncryptKey Zero { get; } = new byte[KeySize];
        
        // Methods.
        /// <summary>
        /// Runs XOR encryption on the input bytes, encrypting it if it
        /// hasn't already been, and decrypting it if it has, using the key provided.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted/decrypted data</returns>
        public void EncryptDecrypt(Span<byte> data)
        {
            for (var i = 0; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ byteKey[i % byteKey.Length]);
        }
        public bool Equals(XorEncryptKey other) => ByteArrayComparer.Current.Equals(byteKey, other.byteKey);
        public override bool Equals(object? obj) => obj is XorEncryptKey other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteKey);
        public byte[] ToByteArray() => (byte[])byteKey.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteKey;
        public override string ToString() => byteKey.ToHex();
        
        // Static methods.
        public static XorEncryptKey FromByteArray(byte[] value) => new(value);
        public static XorEncryptKey FromString(string value) => new(value);
        public static bool IsValidKey(byte[] value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            return value.Length == KeySize;
        }
        public static bool IsValidKey(string value)
        {
            try
            {
                return IsValidKey(value.HexToByteArray());
            }
            catch (FormatException)
            {
                return false;
            }
        }
        
        // Operator methods.
        public static bool operator ==(XorEncryptKey left, XorEncryptKey right) => left.Equals(right);
        public static bool operator !=(XorEncryptKey left, XorEncryptKey right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator XorEncryptKey(string value) => new(value);
        public static implicit operator XorEncryptKey(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(XorEncryptKey value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(XorEncryptKey value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](XorEncryptKey value) => value.ToByteArray();
    }
}