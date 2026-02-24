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
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(EncryptionKey256TypeConverter))]
    public readonly struct EncryptionKey256 : IEquatable<EncryptionKey256>, IParsable<EncryptionKey256>
    {
        // Consts.
        public const int KeySize = 32;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteKey;

        // Constructor.
        public EncryptionKey256(ReadOnlyMemory<byte> key)
        {
            if (!IsValidKey(key))
                throw new ArgumentOutOfRangeException(nameof(key));
            
            byteKey = key;
        }
        
        public EncryptionKey256(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            
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
        public static EncryptionKey256 BuildNewRandom()
        {
            var keyBytes = new byte[KeySize];
            RandomNumberGenerator.Fill(keyBytes);
            return new EncryptionKey256(keyBytes);
        }
        
        // Static properties.
        public static EncryptionKey256 Zero { get; } = new byte[KeySize];
        
        // Methods.
        /// <summary>
        /// Runs XOR encryption on the input bytes, encrypting it if it
        /// hasn't already been, and decrypting it if it has, using the key provided.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Encrypted/decrypted data</returns>
        public void XorEncryptDecrypt(Span<byte> data)
        {
            for (var i = 0; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ byteKey.Span[i % byteKey.Length]);
        }
        public bool Equals(EncryptionKey256 other) => byteKey.Span.SequenceEqual(other.byteKey.Span);
        public override bool Equals(object? obj) => obj is EncryptionKey256 other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteKey.ToArray());
        public byte[] ToByteArray() => byteKey.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteKey;
        public override string ToString() => byteKey.ToArray().ToHex();
        
        // Static methods.
        public static EncryptionKey256 FromByteArray(byte[] value) => new(value);
        public static EncryptionKey256 FromString(string value) => new(value);
        public static bool IsValidKey(ReadOnlyMemory<byte> value) => value.Length == KeySize;
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
        public static EncryptionKey256 Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out EncryptionKey256 result)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                result = default;
                return false;
            }

#pragma warning disable CA1031
            try
            {
                result = FromString(s);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
#pragma warning restore CA1031
        }
        
        // Operator methods.
        public static bool operator ==(EncryptionKey256 left, EncryptionKey256 right) => left.Equals(right);
        public static bool operator !=(EncryptionKey256 left, EncryptionKey256 right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator EncryptionKey256(string value) => new(value);
        public static implicit operator EncryptionKey256(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(EncryptionKey256 value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(EncryptionKey256 value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](EncryptionKey256 value) => value.ToByteArray();
    }
}