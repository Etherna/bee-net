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
    public readonly struct SwarmHash : IEquatable<SwarmHash>
    {
        // Consts.
        public const int HashSize = 32; //Keccak hash size
        
        // Fields.
        private readonly byte[] byteHash;

        // Constructors.
        public SwarmHash(byte[] hash)
        {
            ArgumentNullException.ThrowIfNull(hash, nameof(hash));
            if (hash.Length != HashSize)
                throw new ArgumentOutOfRangeException(nameof(hash));
            
            byteHash = hash;
        }

        public SwarmHash(string hash)
        {
            ArgumentNullException.ThrowIfNull(hash, nameof(hash));
            
            byteHash = hash.HexToByteArray();
            
            if (byteHash.Length != HashSize)
                throw new ArgumentOutOfRangeException(nameof(hash));
        }
        
        // Static properties.
        public static SwarmHash Zero { get; } = new byte[HashSize];

        // Methods.
        public bool Equals(SwarmHash other) => ByteArrayComparer.Current.Equals(byteHash, other.byteHash);
        public override bool Equals(object? obj) => obj is SwarmHash other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteHash);
        public uint ToBucketId() =>
            BinaryPrimitives.ReadUInt32BigEndian(byteHash.AsSpan()[..4]) >> (32 - PostageBatch.BucketDepth);
        public byte[] ToByteArray() => (byte[])byteHash.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteHash;
        public override string ToString() => byteHash.ToHex();
        
        // Static methods.
        public static SwarmHash FromByteArray(byte[] value) => new(value);
        public static SwarmHash FromString(string value) => new(value);
        
        // Operator methods.
        public static bool operator ==(SwarmHash left, SwarmHash right) => left.Equals(right);
        public static bool operator !=(SwarmHash left, SwarmHash right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmHash(string value) => new(value);
        public static implicit operator SwarmHash(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmHash value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(SwarmHash value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](SwarmHash value) => value.ToByteArray();
    }
}