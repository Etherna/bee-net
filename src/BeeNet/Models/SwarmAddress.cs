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

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;

namespace Etherna.BeeNet.Models
{
    public readonly struct SwarmAddress : IEquatable<SwarmAddress>
    {
        // Consts.
        public const int HashByteSize = 32;
        
        // Fields.
        private readonly byte[] byteAddress;

        // Constructors.
        public SwarmAddress(byte[] byteAddress)
        {
            ArgumentNullException.ThrowIfNull(byteAddress, nameof(byteAddress));
            if (byteAddress.Length != HashByteSize)
                throw new ArgumentOutOfRangeException(nameof(byteAddress));
            
            this.byteAddress = byteAddress;
        }

        public SwarmAddress(string strAddress)
        {
            ArgumentNullException.ThrowIfNull(strAddress, nameof(strAddress));
            
            byteAddress = strAddress.HexToByteArray();
            
            if (byteAddress.Length != HashByteSize)
                throw new ArgumentOutOfRangeException(nameof(strAddress));
        }
        
        // Static properties.
        public static SwarmAddress Zero { get; } = new(new byte[16]);

        // Methods.
        public bool Equals(SwarmAddress other) => ByteArrayComparer.Current.Equals(byteAddress, other.byteAddress);
        public override bool Equals(object? obj) => obj is SwarmAddress other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteAddress);
        public byte[] ToByteArray() => (byte[])byteAddress.Clone();
        public override string ToString() => byteAddress.ToHex();
        
        // Static methods.
        public static SwarmAddress FromByteArray(byte[] value) => new(value);
        public static SwarmAddress FromString(string value) => new(value);
        
        // Operator methods.
        public static bool operator ==(SwarmAddress left, SwarmAddress right) => left.Equals(right);
        public static bool operator !=(SwarmAddress left, SwarmAddress right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmAddress(string value) => new(value);
        public static implicit operator SwarmAddress(byte[] value) => new(value);
        
        public static explicit operator string(SwarmAddress value) => value.ToString();
        public static explicit operator byte[](SwarmAddress value) => value.ToByteArray();
    }
}