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
    [TypeConverter(typeof(SwarmOverlayAddressTypeConverter))]
    public readonly struct SwarmOverlayAddress : IEquatable<SwarmOverlayAddress>, IParsable<SwarmOverlayAddress>
    {
        // Consts.
        public const int AddressSize = 32;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteAddress;

        // Constructors.
        public SwarmOverlayAddress(ReadOnlyMemory<byte> address)
        {
            if (!IsValidAddress(address))
                throw new ArgumentOutOfRangeException(nameof(address));
            
            byteAddress = address;
        }

        public SwarmOverlayAddress(string address)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            
            try
            {
                byteAddress = address.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid address", nameof(address));
            }
            
            if (!IsValidAddress(byteAddress))
                throw new ArgumentOutOfRangeException(nameof(address));
        }
        
        // Static properties.
        public static SwarmOverlayAddress Zero { get; } = new byte[AddressSize];

        // Methods.
        public bool Equals(SwarmOverlayAddress other) => byteAddress.Span.SequenceEqual(other.byteAddress.Span);
        public override bool Equals(object? obj) => obj is SwarmOverlayAddress other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteAddress.ToArray());
        public byte[] ToByteArray() => byteAddress.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteAddress;
        public override string ToString() => byteAddress.ToArray().ToHex();
        
        // Static methods.
        public static SwarmOverlayAddress FromByteArray(byte[] value) => new(value);
        public static SwarmOverlayAddress FromString(string value) => new(value);
        public static bool IsValidAddress(ReadOnlyMemory<byte> value) => value.Length == AddressSize;
        public static bool IsValidAddress(string value)
        {
            try
            {
                return IsValidAddress(value.HexToByteArray());
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static SwarmOverlayAddress Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SwarmOverlayAddress result)
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
        public static bool operator ==(SwarmOverlayAddress left, SwarmOverlayAddress right) => left.Equals(right);
        public static bool operator !=(SwarmOverlayAddress left, SwarmOverlayAddress right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmOverlayAddress(string value) => new(value);
        public static implicit operator SwarmOverlayAddress(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmOverlayAddress value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(SwarmOverlayAddress value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](SwarmOverlayAddress value) => value.ToByteArray();
    }
}