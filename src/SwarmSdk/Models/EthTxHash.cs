// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
// 
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.SwarmSdk.Extensions;
using Etherna.SwarmSdk.TypeConverters;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.SwarmSdk.Models
{
    [TypeConverter(typeof(EthTxHashTypeConverter))]
    public readonly struct EthTxHash : IEquatable<EthTxHash>, IParsable<EthTxHash>
    {
        // Consts.
        public const int HashSize = 32;
        
        // Fields.
        private readonly byte[] byteHash;
        
        // Constructors.
        public EthTxHash(byte[] hash)
        {
            ArgumentNullException.ThrowIfNull(hash);
            if (!IsValidHash(hash))
                throw new ArgumentOutOfRangeException(nameof(hash));

            byteHash = hash;
        }

        public EthTxHash(string hash)
        {
            ArgumentNullException.ThrowIfNull(hash);
            
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
        
        // Static properties.
        public static EthTxHash Zero { get; } = new byte[HashSize];

        // Methods.
        public bool Equals(EthTxHash other) => byteHash.AsSpan().SequenceEqual(other.byteHash);
        public override bool Equals(object? obj) => obj is EthTxHash other && Equals(other);
        public override int GetHashCode() => byteHash.AsSpan().ToHashCode();
        public byte[] ToByteArray() => (byte[])byteHash.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteHash.AsMemory();
        public override string ToString() => byteHash.ToHex(prefix: true);
        
        // Static methods.
        public static EthTxHash FromByteArray(byte[] value) => new(value);
        public static EthTxHash FromString(string value) => new(value);
        public static bool IsValidHash(byte[] value)
        {
            ArgumentNullException.ThrowIfNull(value);
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
        public static EthTxHash Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out EthTxHash result)
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