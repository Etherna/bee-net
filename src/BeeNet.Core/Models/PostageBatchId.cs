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
    [TypeConverter(typeof(PostageBatchIdTypeConverter))]
    public readonly struct PostageBatchId : IEquatable<PostageBatchId>, IParsable<PostageBatchId>
    {
        // Consts.
        public const int BatchIdSize = 32;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteId;

        // Constructors.
        public PostageBatchId(ReadOnlyMemory<byte> batchId)
        {
            if (batchId.Length != BatchIdSize)
                throw new ArgumentOutOfRangeException(nameof(batchId));
            
            byteId = batchId;
        }
        
        public PostageBatchId(string batchId)
        {
            ArgumentNullException.ThrowIfNull(batchId, nameof(batchId));
            
            byteId = batchId.HexToByteArray();
            
            if (byteId.Length != BatchIdSize)
                throw new ArgumentOutOfRangeException(nameof(batchId));
        }
        
        // Static properties.
        public static PostageBatchId Zero { get; } = new byte[BatchIdSize];

        // Methods.
        public bool Equals(PostageBatchId other) => byteId.Span.SequenceEqual(other.byteId.Span);
        public override bool Equals(object? obj) => obj is PostageBatchId other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteId.ToArray());
        public byte[] ToByteArray() => byteId.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteId;
        public override string ToString() => byteId.ToArray().ToHex();
        
        // Static methods.
        public static PostageBatchId FromByteArray(byte[] value) => new(value);
        public static PostageBatchId FromString(string value) => new(value);
        public static PostageBatchId Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out PostageBatchId result)
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
        public static bool operator ==(PostageBatchId left, PostageBatchId right) => left.Equals(right);
        public static bool operator !=(PostageBatchId left, PostageBatchId right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator PostageBatchId(string value) => new(value);
        public static implicit operator PostageBatchId(byte[] value) => new(value);
        
        public static explicit operator byte[](PostageBatchId value) => value.ToByteArray();
        public static explicit operator ReadOnlyMemory<byte>(PostageBatchId value) => value.ToReadOnlyMemory();
        public static explicit operator string(PostageBatchId value) => value.ToString();
    }
}