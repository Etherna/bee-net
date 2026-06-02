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

using Etherna.SwarmSdk.Models;
using System;
using System.Linq;
using Xunit;

namespace Etherna.SwarmSdk.Hashing.Signer
{
    public class PrivateKeySignerTest
    {
        // Well-known deterministic test account (Ganache account #0).
        // Signing is deterministic (RFC 6979), so these golden values are stable and must
        // remain byte-for-byte identical.
        private const string PrivateKeyHex = "0x4f3edf983ac636a65a842ce7c78d9aa706d3b113bce9c46f30d7d21715b23b1d";
        private const string ExpectedAddress = "0x90F8bf6A479f320ead074411a4B0e7944Ea8c9C1";

        // Uppercase hex (System.Convert.ToHexString format), no prefix.
        private const string ExpectedPublicKeyHex =
            "E68ACFC0253A10620DFF706B0A1B1F1F5833EA3BEB3BDE2250D5F271F3563606672EBC45E0B7EA2E816ECB70CA03137B1C9476EEC63D4632E990020B7B6FBA39";
        private const string ExpectedSignatureHex =
            "FFA8FBB32B6732B56F85827602493D1BCB62B0F3DAB1F6A06EFD94A3405E5B4C3217556A027AE67C07CB6FAF871D288AF3F20AFE4C58379D344FDAAD364F172A1B";

        // A fixed 32-byte digest to sign.
        private static byte[] Digest => Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();

        // Tests.
        [Fact]
        public void PublicAddressMatchesKnownAccount()
        {
            var signer = new PrivateKeySigner(new EthPrivateKey(PrivateKeyHex));

            Assert.Equal(ExpectedAddress, signer.PublicAddress.ToString());
        }

        [Fact]
        public void PublicKeyIsStable()
        {
            var signer = new PrivateKeySigner(new EthPrivateKey(PrivateKeyHex));

            var publicKey = signer.GetPublicKey();

            Assert.Equal(64, publicKey.Length);
            Assert.Equal(ExpectedPublicKeyHex, Convert.ToHexString(publicKey));
        }

        [Fact]
        public void SignatureIsDeterministicAndStable()
        {
            var signer = new PrivateKeySigner(new EthPrivateKey(PrivateKeyHex));

            var signature = signer.Sign(Digest);

            Assert.Equal(65, signature.Length);
            Assert.Equal(ExpectedSignatureHex, Convert.ToHexString(signature));
        }

        [Fact]
        public void SignedDigestRecoversToSignerAddress()
        {
            var signer = new PrivateKeySigner(new EthPrivateKey(PrivateKeyHex));

            var signature = signer.Sign(Digest);
            var recovered = new SwarmSocSignature(signature).RecoverOwner(Digest);

            Assert.Equal(signer.PublicAddress, recovered);
        }
    }
}
