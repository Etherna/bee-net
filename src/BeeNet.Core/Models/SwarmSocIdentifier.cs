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

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(SwarmSocIdentifierTypeConverter))]
    public readonly struct SwarmSocIdentifier : IEquatable<SwarmSocIdentifier>, IParsable<SwarmSocIdentifier>
    {
        // Consts.
        public const int IdentifierSize = SwarmHash.HashSize;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteIdentifier;

        // Constructors.
        public SwarmSocIdentifier(ReadOnlyMemory<byte> identifier)
        {
            if (!IsValidIdentifier(identifier))
                throw new ArgumentOutOfRangeException(nameof(identifier));
            
            byteIdentifier = identifier;
        }

        public SwarmSocIdentifier(string identifier)
        {
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));
            
            try
            {
                byteIdentifier = identifier.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hex", nameof(identifier));
            }
            
            if (!IsValidIdentifier(byteIdentifier))
                throw new ArgumentOutOfRangeException(nameof(identifier));
        }

        // Methods.
        public bool Equals(SwarmSocIdentifier other) => byteIdentifier.Span.SequenceEqual(other.byteIdentifier.Span);
        public override bool Equals(object? obj) => obj is SwarmSocIdentifier other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteIdentifier.ToArray());
        public byte[] ToByteArray() => byteIdentifier.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteIdentifier;
        public override string ToString() => byteIdentifier.ToArray().ToHex();
        
        // Static methods.
        public static SwarmSocIdentifier FromByteArray(byte[] value) => new(value);
        public static SwarmSocIdentifier FromString(string value) => new(value);
        public static bool IsValidIdentifier(ReadOnlyMemory<byte> value) => value.Length == IdentifierSize;
        public static bool IsValidIdentifier(string value)
        {
            try
            {
                return IsValidIdentifier(value.HexToByteArray());
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static SwarmSocIdentifier Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SwarmSocIdentifier result)
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
        public static bool operator ==(SwarmSocIdentifier left, SwarmSocIdentifier right) => left.Equals(right);
        public static bool operator !=(SwarmSocIdentifier left, SwarmSocIdentifier right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmSocIdentifier(string value) => new(value);
        public static implicit operator SwarmSocIdentifier(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmSocIdentifier value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(SwarmSocIdentifier value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](SwarmSocIdentifier value) => value.ToByteArray();
    }
}