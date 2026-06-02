// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
//
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.SwarmSdk.Extensions;
using Etherna.SwarmSdk.Hashing;
using Etherna.SwarmSdk.Hashing.Signer;
using System;

namespace Etherna.SwarmSdk.Models
{
    /// <summary>
    /// A secp256k1 private key (32 bytes), able to derive its public key/address and to produce
    /// EIP-191 personal-message signatures.
    /// </summary>
    public sealed class EthPrivateKey
    {
        // Consts.
        public const int KeySize = Secp256k1.PrivateKeySize;

        // Fields.
        private readonly byte[] bytes;

        // Constructors.
        public EthPrivateKey(byte[] privateKey)
        {
            ArgumentNullException.ThrowIfNull(privateKey);
            if (privateKey.Length != KeySize)
                throw new ArgumentOutOfRangeException(nameof(privateKey));

            bytes = (byte[])privateKey.Clone();
        }

        public EthPrivateKey(string privateKey)
        {
            ArgumentNullException.ThrowIfNull(privateKey);

            byte[] parsed;
            try
            {
                parsed = privateKey.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hex", nameof(privateKey));
            }

            if (parsed.Length != KeySize)
                throw new ArgumentOutOfRangeException(nameof(privateKey));

            bytes = parsed;
        }

        // Properties.
        public EthAddress Address => PublicKey.ToAddress();
        public EthPublicKey PublicKey => new(Secp256k1.GetPublicKey(bytes));

        // Methods.
        /// <summary>
        /// Sign a digest with the Ethereum personal-message prefix (EIP-191, type 0x45).
        /// </summary>
        /// <param name="digest">The digest to sign (the EIP-191 prefix is applied internally)</param>
        /// <param name="hasher">Optional Keccak hasher to reuse; must not be shared across threads</param>
        /// <returns>A 65-byte r||s||v signature</returns>
        public byte[] Sign(ReadOnlySpan<byte> digest, Hasher? hasher = null) => Secp256k1.Sign(digest, bytes, hasher);
    }
}
