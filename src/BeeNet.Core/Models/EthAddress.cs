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
    [TypeConverter(typeof(EthAddressTypeConverter))]
    public readonly struct EthAddress : IEquatable<EthAddress>
    {
        // Consts.
        public const int AddressSize = 20;
        
        // Fields.
        private readonly byte[] byteAddress;
        
        // Constructors.
        public EthAddress(byte[] address)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            if (!IsValidAddress(address))
                throw new ArgumentOutOfRangeException(nameof(address));

            byteAddress = address;
        }

        public EthAddress(string address)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            if (!IsValidAddress(address))
                throw new ArgumentOutOfRangeException(nameof(address));

            byteAddress = address.HexToByteArray();
        }
        
        // Static properties.
        public static EthAddress Zero { get; } = AddressUtil.ZERO_ADDRESS;

        // Methods.
        public bool Equals(EthAddress other) => ByteArrayComparer.Current.Equals(byteAddress, other.byteAddress);
        public override bool Equals(object? obj) => obj is EthAddress other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteAddress);
        public byte[] ToByteArray() => (byte[])byteAddress.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteAddress.AsMemory();
        public override string ToString() => byteAddress.ConvertToEthereumChecksumAddress();
        
        // Static methods.
        public static EthAddress FromByteArray(byte[] value) => new(value);
        public static EthAddress FromString(string value) => new(value);
        public static bool IsValidAddress(byte[] value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            return value.Length == AddressSize;
        }
        public static bool IsValidAddress(string value) =>
            //accept as valid both with "0x..." or not
            value.IsHex() &&
            value.IsValidEthereumAddressLength();
        
        // Operator methods.
        public static bool operator ==(EthAddress left, EthAddress right) => left.Equals(right);
        public static bool operator !=(EthAddress left, EthAddress right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator EthAddress(string value) => new(value);
        public static implicit operator EthAddress(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(EthAddress value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(EthAddress value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](EthAddress value) => value.ToByteArray();
    }
}