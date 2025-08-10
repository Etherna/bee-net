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
using System.Buffers.Binary;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(SwarmHashTypeConverter))]
    public readonly struct SwarmHash : IEquatable<SwarmHash>, IParsable<SwarmHash>
    {
        // Consts.
        public const int HashSize = 32;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteHash;

        // Constructors.
        public SwarmHash(ReadOnlyMemory<byte> hash)
        {
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
        public bool Equals(SwarmHash other) => byteHash.Span.SequenceEqual(other.byteHash.Span);
        public override bool Equals(object? obj) => obj is SwarmHash other && Equals(other);
        public byte[] GetDistanceBytesFrom(SwarmHash other) => GetDistanceBytes(byteHash.Span, other.byteHash.Span);
        public byte[] GetDistanceBytesFrom(SwarmOverlayAddress other) => GetDistanceBytes(byteHash.Span, other.ToReadOnlyMemory().Span);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteHash.ToArray());
        public ushort ToBucketId() => BinaryPrimitives.ReadUInt16BigEndian(byteHash.Span[..2]);
        public byte[] ToByteArray() => byteHash.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteHash;
        public override string ToString() => byteHash.ToArray().ToHex();
        
        // Static methods.
        /// <summary>
        /// Compare x and y to the target in terms of bytes distance.
        /// </summary>
        /// <returns>
        /// Returns -1 if x is closer to target than y, 0 if x and y are equals, 1 if x is farther than y
        /// </returns>
        public static int CompareDistances(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y, ReadOnlySpan<byte> target)
        {
            for (int i = 0; i < HashSize; i++)
            {
                var xDist = x[i] ^ target[i];
                var yDist = y[i] ^ target[i];
                if (xDist < yDist)
                    return -1;
                if (xDist > yDist)
                    return 1;
            }
            return 0;
        }
        public static SwarmHash FromByteArray(byte[] value) => new(value);
        public static SwarmHash FromString(string value) => new(value);
        public static byte[] GetDistanceBytes(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            var distance = new byte[HashSize];
            for (int i = 0; i < HashSize; i++)
                distance[i] = (byte)(x[i] ^ y[i]);
            
            return distance;
        }
        public static bool IsValidHash(ReadOnlyMemory<byte> value) => value.Length == HashSize;
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
        public static SwarmHash Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SwarmHash result)
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