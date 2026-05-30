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
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.Globalization;
using System.Text;

namespace Etherna.BeeNet.Hashing.Signer
{
    /// <summary>
    /// secp256k1 primitives over BouncyCastle, reproducing Ethereum's EIP-191 personal-message
    /// signing/recovery and EIP-55 address checksum. Signing is deterministic (RFC 6979).
    /// </summary>
    internal static class Secp256k1
    {
        // Consts.
        public const int PrivateKeySize = 32;
        public const int PublicKeySize = 64;
        public const int SignatureSize = 65;
        private const int AddressSize = 20;
        private const byte RecoveryIdOffset = 27;
        private const byte Eip191Version = 0x19;

        // Fields.
        private static readonly X9ECParameters CurveParameters = SecNamedCurves.GetByName("secp256k1");
        private static readonly ECDomainParameters Domain =
            new(CurveParameters.Curve, CurveParameters.G, CurveParameters.N, CurveParameters.H);
        private static readonly BigInteger Order = Domain.N;
        private static readonly BigInteger HalfOrder = Domain.N.ShiftRight(1);
        private static readonly BigInteger Prime = Domain.Curve.Field.Characteristic;
        private static readonly byte[] Eip191Prefix = BuildEip191Prefix();

        // Methods.
        /// <summary>
        /// Derive the 64-byte uncompressed public key (without the 0x04 prefix) from a private key.
        /// </summary>
        public static byte[] GetPublicKey(ReadOnlySpan<byte> privateKey)
        {
            if (privateKey.Length != PrivateKeySize)
                throw new ArgumentOutOfRangeException(nameof(privateKey));

            var d = new BigInteger(1, privateKey.ToArray());
            return Domain.G.Multiply(d).Normalize().GetEncoded(false)[1..];
        }

        /// <summary>
        /// Derive the 20-byte address from a 64-byte uncompressed public key.
        /// </summary>
        public static byte[] PublicKeyToAddressBytes(ReadOnlySpan<byte> publicKey, Hasher? hasher = null)
        {
            if (publicKey.Length != PublicKeySize)
                throw new ArgumentOutOfRangeException(nameof(publicKey));

            return (hasher ?? new Hasher()).ComputeHash(publicKey)[12..];
        }

        /// <summary>
        /// Recover the 64-byte public key from a 65-byte r||s||v signature over an EIP-191
        /// prefixed digest.
        /// </summary>
        public static byte[] RecoverPublicKey(ReadOnlySpan<byte> digest, ReadOnlySpan<byte> signature, Hasher? hasher = null)
        {
            if (signature.Length != SignatureSize)
                throw new ArgumentOutOfRangeException(nameof(signature));

            var r = new BigInteger(1, signature[..32].ToArray());
            var s = new BigInteger(1, signature.Slice(32, 32).ToArray());
            var recoveryId = signature[64] - RecoveryIdOffset;
            if (recoveryId is < 0 or > 3)
                throw new ArgumentException("Invalid signature recovery id", nameof(signature));

            var hash = HashEthereumMessage(digest, hasher);
            var point = RecoverPoint(recoveryId, r, s, hash)
                ?? throw new ArgumentException("Cannot recover public key from signature", nameof(signature));

            return point.GetEncoded(false)[1..];
        }

        /// <summary>
        /// Sign an EIP-191 prefixed digest, returning a 65-byte r||s||v signature (v = recId + 27).
        /// </summary>
        public static byte[] Sign(ReadOnlySpan<byte> digest, ReadOnlySpan<byte> privateKey, Hasher? hasher = null)
        {
            if (privateKey.Length != PrivateKeySize)
                throw new ArgumentOutOfRangeException(nameof(privateKey));

            var d = new BigInteger(1, privateKey.ToArray());
            var hash = HashEthereumMessage(digest, hasher);

            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            signer.Init(true, new ECPrivateKeyParameters(d, Domain));
            var components = signer.GenerateSignature(hash);
            var r = components[0];
            var s = components[1];

            // Canonicalize to low-s.
            if (s.CompareTo(HalfOrder) > 0)
                s = Order.Subtract(s);

            var recoveryId = CalculateRecoveryId(r, s, hash, d);

            var signature = new byte[SignatureSize];
            WriteBigEndianFixed(r, signature.AsSpan(0, 32));
            WriteBigEndianFixed(s, signature.AsSpan(32, 32));
            signature[64] = (byte)(recoveryId + RecoveryIdOffset);
            return signature;
        }

        /// <summary>
        /// Encode a 20-byte address as an EIP-55 mixed-case checksum string with "0x" prefix.
        /// </summary>
        public static string ToChecksumAddress(ReadOnlySpan<byte> address, Hasher? hasher = null)
        {
            if (address.Length != AddressSize)
                throw new ArgumentOutOfRangeException(nameof(address));

            var lower = address.ToHex();
            var hash = (hasher ?? new Hasher()).ComputeHash(Encoding.ASCII.GetBytes(lower));

            var chars = new char[2 + (AddressSize * 2)];
            chars[0] = '0';
            chars[1] = 'x';
            for (var i = 0; i < AddressSize * 2; i++)
            {
                var c = lower[i];
                var nibble = (i & 1) == 0 ? hash[i >> 1] >> 4 : hash[i >> 1] & 0xF;
                chars[2 + i] = nibble > 7 ? char.ToUpperInvariant(c) : c;
            }

            return new string(chars);
        }

        // Helpers.
        private static byte[] BuildEip191Prefix()
        {
            var tag = "Ethereum Signed Message:\n"u8.ToArray();
            var prefix = new byte[1 + tag.Length];
            prefix[0] = Eip191Version;
            tag.CopyTo(prefix, 1);
            return prefix;
        }

        private static int CalculateRecoveryId(BigInteger r, BigInteger s, byte[] hash, BigInteger d)
        {
            var expected = Domain.G.Multiply(d).Normalize().GetEncoded(true);
            for (var recoveryId = 0; recoveryId < 4; recoveryId++)
            {
                var point = RecoverPoint(recoveryId, r, s, hash);
                if (point is not null && point.GetEncoded(true).AsSpan().SequenceEqual(expected))
                    return recoveryId;
            }

            throw new InvalidOperationException("Cannot compute a recovery id for the signature");
        }

        private static ECPoint DecompressPoint(BigInteger x, bool yBit)
        {
            var encoded = new byte[33];
            encoded[0] = (byte)(yBit ? 0x03 : 0x02);
            WriteBigEndianFixed(x, encoded.AsSpan(1, 32));
            return Domain.Curve.DecodePoint(encoded);
        }

        private static byte[] HashEthereumMessage(ReadOnlySpan<byte> message, Hasher? hasher)
        {
            var lengthText = message.Length.ToString(CultureInfo.InvariantCulture);
            var prefixLength = Eip191Prefix.Length + lengthText.Length;

            var buffer = new byte[prefixLength + message.Length];
            Eip191Prefix.CopyTo(buffer.AsSpan());
            Encoding.ASCII.GetBytes(lengthText, buffer.AsSpan(Eip191Prefix.Length));
            message.CopyTo(buffer.AsSpan(prefixLength));

            return (hasher ?? new Hasher()).ComputeHash(buffer);
        }

        private static ECPoint? RecoverPoint(int recoveryId, BigInteger r, BigInteger s, byte[] messageHash)
        {
            // x = r + (recoveryId / 2) * n
            var i = BigInteger.ValueOf(recoveryId / 2);
            var x = r.Add(i.Multiply(Order));
            if (x.CompareTo(Prime) >= 0)
                return null;

            var rPoint = DecompressPoint(x, (recoveryId & 1) == 1);

            // DecodePoint validates the point lies on secp256k1, whose cofactor is 1: every
            // on-curve point therefore has order n, so the usual n*R == O check is always true
            // and is skipped here (it would cost a full scalar multiplication per candidate):
            //
            //     if (!rPoint.Multiply(Order).IsInfinity)
            //         return null;

            var e = new BigInteger(1, messageHash);
            var eInv = BigInteger.Zero.Subtract(e).Mod(Order);
            var rInv = r.ModInverse(Order);
            var srInv = rInv.Multiply(s).Mod(Order);
            var eInvrInv = rInv.Multiply(eInv).Mod(Order);

            return ECAlgorithms.SumOfTwoMultiplies(Domain.G, eInvrInv, rPoint, srInv).Normalize();
        }

        private static void WriteBigEndianFixed(BigInteger value, Span<byte> destination)
        {
            var bytes = value.ToByteArrayUnsigned();
            if (bytes.Length > destination.Length)
                throw new InvalidOperationException("Value does not fit in destination");

            destination.Clear();
            bytes.CopyTo(destination[^bytes.Length..]);
        }
    }
}
