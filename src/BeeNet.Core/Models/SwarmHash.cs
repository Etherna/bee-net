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
            if (!IsValidHash(hash))
                throw new ArgumentOutOfRangeException(nameof(hash));
            
            byteHash = hash;
        }

        public SwarmHash(string hash)
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
        
        // Static properties.
        public static SwarmHash Zero { get; } = new byte[HashSize];

        // Methods.
        public bool Equals(SwarmHash other) => ByteArrayComparer.Current.Equals(
            byteHash ?? Zero.byteHash, other.byteHash);
        public override bool Equals(object? obj) => obj is SwarmHash other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteHash ?? Zero.byteHash);
        public uint ToBucketId() => BinaryPrimitives.ReadUInt32BigEndian(
            (byteHash ?? Zero.byteHash).AsSpan()[..4]) >> (32 - PostageBatch.BucketDepth);
        public byte[] ToByteArray() => (byte[])(byteHash ?? Zero.byteHash).Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteHash ?? Zero.byteHash;
        public override string ToString() => (byteHash ?? Zero.byteHash).ToHex();
        
        // Static methods.
        public static SwarmHash FromByteArray(byte[] value) => new(value);
        public static SwarmHash FromString(string value) => new(value);
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