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
using System.Buffers.Binary;

namespace Etherna.BeeNet.Models
{
    public readonly struct SwarmAddress : IEquatable<SwarmAddress>
    {
        // Consts.
        public const int HashSize = 32; //Keccak hash size
        
        // Fields.
        private readonly byte[] byteAddress;

        // Constructors.
        public SwarmAddress(byte[] address)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            if (address.Length != HashSize)
                throw new ArgumentOutOfRangeException(nameof(address));
            
            byteAddress = address;
        }

        public SwarmAddress(string address)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            
            byteAddress = address.HexToByteArray();
            
            if (byteAddress.Length != HashSize)
                throw new ArgumentOutOfRangeException(nameof(address));
        }
        
        // Static properties.
        public static SwarmAddress Zero { get; } = new byte[HashSize];

        // Methods.
        public bool Equals(SwarmAddress other) => ByteArrayComparer.Current.Equals(byteAddress, other.byteAddress);
        public override bool Equals(object? obj) => obj is SwarmAddress other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteAddress);
        public uint ToBucketId() =>
            BinaryPrimitives.ReadUInt32BigEndian(byteAddress.AsSpan()[..4]) >> (32 - PostageBatch.BucketDepth);
        public byte[] ToByteArray() => (byte[])byteAddress.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteAddress;
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
        public static explicit operator ReadOnlyMemory<byte>(SwarmAddress value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](SwarmAddress value) => value.ToByteArray();
    }
}