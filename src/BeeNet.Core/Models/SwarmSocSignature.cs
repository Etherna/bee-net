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
using Nethereum.Signer;
using Nethereum.Util;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(SwarmSocSignatureTypeConverter))]
    public readonly struct SwarmSocSignature : IEquatable<SwarmSocSignature>, IParsable<SwarmSocSignature>
    {
        // Consts.
        public const int SignatureSize = 65;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteSignature;

        // Constructors.
        public SwarmSocSignature(ReadOnlyMemory<byte> signature)
        {
            if (!IsValidSignature(signature))
                throw new ArgumentOutOfRangeException(nameof(signature));
            
            byteSignature = signature;
        }

        public SwarmSocSignature(string signature)
        {
            ArgumentNullException.ThrowIfNull(signature);
            
            try
            {
                byteSignature = signature.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hex", nameof(signature));
            }
            
            if (!IsValidSignature(byteSignature))
                throw new ArgumentOutOfRangeException(nameof(signature));
        }

        // Methods.
        public bool Equals(SwarmSocSignature other) => byteSignature.Span.SequenceEqual(other.byteSignature.Span);
        public override bool Equals(object? obj) => obj is SwarmSocSignature other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteSignature.ToArray());
        public EthAddress RecoverOwner(byte[] toSignDigest)
        {
            var signer = new EthereumMessageSigner();
            return signer.EcRecover(toSignDigest, ToString());
        }
        public byte[] ToByteArray() => byteSignature.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteSignature;
        public override string ToString() => byteSignature.ToArray().ToHex();
        
        // Static methods.
        public static SwarmSocSignature FromByteArray(byte[] value) => new(value);
        public static SwarmSocSignature FromString(string value) => new(value);
        public static bool IsValidSignature(ReadOnlyMemory<byte> value) => value.Length == SignatureSize;
        public static bool IsValidSignature(string value)
        {
            try
            {
                return IsValidSignature(value.HexToByteArray());
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static SwarmSocSignature Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SwarmSocSignature result)
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
        public static bool operator ==(SwarmSocSignature left, SwarmSocSignature right) => left.Equals(right);
        public static bool operator !=(SwarmSocSignature left, SwarmSocSignature right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmSocSignature(string value) => new(value);
        public static implicit operator SwarmSocSignature(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmSocSignature value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(SwarmSocSignature value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](SwarmSocSignature value) => value.ToByteArray();
    }
}