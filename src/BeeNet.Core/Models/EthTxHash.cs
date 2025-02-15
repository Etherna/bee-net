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

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(EthTxHashTypeConverter))]
    public readonly struct EthTxHash : IEquatable<EthTxHash>
    {
        // Consts.
        public const int HashSize = 32;
        
        // Fields.
        private readonly byte[] byteHash;
        
        // Constructors.
        public EthTxHash(byte[] hash)
        {
            ArgumentNullException.ThrowIfNull(hash, nameof(hash));
            if (!IsValidHash(hash))
                throw new ArgumentOutOfRangeException(nameof(hash));

            byteHash = hash;
        }

        public EthTxHash(string hash)
        {
            ArgumentNullException.ThrowIfNull(hash, nameof(hash));
            
            try
            {
                byteHash = hash.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hash", nameof(hash));
            }
            
            if (!IsValidHash(byteHash))
                throw new ArgumentOutOfRangeException(nameof(hash));
        }

        // Methods.
        public bool Equals(EthTxHash other) => ByteArrayComparer.Current.Equals(byteHash, other.byteHash);
        public override bool Equals(object? obj) => obj is EthTxHash other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteHash);
        public byte[] ToByteArray() => (byte[])byteHash.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteHash.AsMemory();
        public override string ToString() => byteHash.ToHex(prefix: true);
        
        // Static methods.
        public static EthTxHash FromByteArray(byte[] value) => new(value);
        public static EthTxHash FromString(string value) => new(value);
        public static bool IsValidHash(byte[] value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            return value.Length == HashSize;
        }
        public static bool IsValidHash(string value)
        {
            try
            {
                return IsValidHash(value.HexToByteArray());
            }
            catch (FormatException)
            {
                return false;
            }
        }
        
        // Operator methods.
        public static bool operator ==(EthTxHash left, EthTxHash right) => left.Equals(right);
        public static bool operator !=(EthTxHash left, EthTxHash right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator EthTxHash(string value) => new(value);
        public static implicit operator EthTxHash(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(EthTxHash value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(EthTxHash value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](EthTxHash value) => value.ToByteArray();
    }
}