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

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Hashing.Signer;
using System;

namespace Etherna.BeeNet.Models
{
    /// <summary>
    /// A secp256k1 public key, as the 64-byte uncompressed point without the 0x04 prefix.
    /// </summary>
    public readonly struct EthPublicKey : IEquatable<EthPublicKey>
    {
        // Consts.
        public const int KeySize = Secp256k1.PublicKeySize;

        // Fields.
        private readonly byte[] bytes;

        // Constructors.
        public EthPublicKey(byte[] publicKey)
        {
            ArgumentNullException.ThrowIfNull(publicKey);
            if (publicKey.Length != KeySize)
                throw new ArgumentOutOfRangeException(nameof(publicKey));

            bytes = publicKey;
        }

        // Methods.
        public bool Equals(EthPublicKey other) => bytes.AsSpan().SequenceEqual(other.bytes);
        public override bool Equals(object? obj) => obj is EthPublicKey other && Equals(other);
        public override int GetHashCode() => bytes.AsSpan().ToHashCode();
        public EthAddress ToAddress(Hasher? hasher = null) => new(Secp256k1.PublicKeyToAddressBytes(bytes, hasher));
        public byte[] ToByteArray() => (byte[])bytes.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => bytes;
        public override string ToString() => bytes.ToHex();

        // Static methods.
        /// <summary>
        /// Recover the public key that produced an EIP-191 signature over the given digest.
        /// </summary>
        /// <param name="digest">The signed digest (the EIP-191 prefix is applied internally)</param>
        /// <param name="signature">The 65-byte r||s||v signature</param>
        /// <param name="hasher">Optional Keccak hasher to reuse; must not be shared across threads</param>
        public static EthPublicKey Recover(ReadOnlySpan<byte> digest, ReadOnlySpan<byte> signature, Hasher? hasher = null) =>
            new(Secp256k1.RecoverPublicKey(digest, signature, hasher));

        // Operator methods.
        public static bool operator ==(EthPublicKey left, EthPublicKey right) => left.Equals(right);
        public static bool operator !=(EthPublicKey left, EthPublicKey right) => !(left == right);
    }
}
