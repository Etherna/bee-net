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

using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Hashing.Signer;
using System;
using System.Linq;
using Xunit;

namespace Etherna.BeeNet.Models
{
    public class SwarmSocTest
    {
        private const string PrivateKeyHex = "0x4f3edf983ac636a65a842ce7c78d9aa706d3b113bce9c46f30d7d21715b23b1d";

        private static SwarmSocIdentifier Identifier =>
            Enumerable.Range(0, SwarmSocIdentifier.IdentifierSize).Select(i => (byte)i).ToArray();

        // Tests.
        [Fact]
        public void ReplicasOwnerIsStable()
        {
            // Documented value, see SwarmSoc.ReplicasOwner.
            Assert.Equal(
                "DC5B20847F43D67928F49CD4F85D696B5A7617B5",
                Convert.ToHexString(SwarmSoc.ReplicasOwner.ToByteArray()));
        }

        [Fact]
        public void SignThenValidateSucceeds()
        {
            var signer = new PrivateKeySigner(new EthPrivateKey(PrivateKeyHex));
            var soc = new SwarmSoc(Identifier, signer.PublicAddress, BuildInnerChunk());

            soc.Sign(signer, new Hasher());

            Assert.True(soc.Signature.HasValue);
            Assert.True(soc.ValidateSoc(new Hasher()));
        }

        [Fact]
        public void SignRecoversToOwner()
        {
            var signer = new PrivateKeySigner(new EthPrivateKey(PrivateKeyHex));
            var soc = new SwarmSoc(Identifier, signer.PublicAddress, BuildInnerChunk());

            soc.Sign(signer, new Hasher());
            var recovered = soc.Signature!.Value.RecoverOwner(soc.ToSignDigest(new Hasher()));

            Assert.Equal(signer.PublicAddress, recovered);
        }

        [Fact]
        public void SignerAndPrivateKeyProduceIdenticalSignature()
        {
            var ecKey = new EthPrivateKey(PrivateKeyHex);
            var signer = new PrivateKeySigner(ecKey);
            var owner = signer.PublicAddress;
            var innerChunk = BuildInnerChunk();

            var bySigner = new SwarmSoc(Identifier, owner, innerChunk);
            bySigner.Sign(signer, new Hasher());

            var byPrivateKey = new SwarmSoc(Identifier, owner, innerChunk);
            byPrivateKey.SignWithPrivateKey(ecKey, new Hasher());

            Assert.Equal(bySigner.Signature!.Value, byPrivateKey.Signature!.Value);
        }

        [Fact]
        public void BuildFromBytesRecoversSignedSoc()
        {
            var bmt = new SwarmChunkBmt();
            var signer = new PrivateKeySigner(new EthPrivateKey(PrivateKeyHex));
            var owner = signer.PublicAddress;
            var innerChunk = BuildInnerChunk(bmt);

            var soc = new SwarmSoc(Identifier, owner, innerChunk);
            soc.Sign(signer, new Hasher());

            var rebuilt = SwarmSoc.BuildFromBytes(null, soc.GetFullPayloadToByteArray(), bmt);

            Assert.Equal(owner, rebuilt.Owner);
            Assert.Equal(soc.Identifier, rebuilt.Identifier);
            Assert.True(rebuilt.Signature.HasValue);
            Assert.Equal(soc.Signature!.Value, rebuilt.Signature!.Value);
            Assert.True(rebuilt.ValidateSoc(new Hasher()));
        }

        // Helpers.
        private static SwarmCac BuildInnerChunk(SwarmChunkBmt? bmt = null)
        {
            bmt ??= new SwarmChunkBmt();
            var data = Enumerable.Range(0, 42).Select(i => (byte)i).ToArray();
            var spanData = SwarmCac.BuildFromData(SwarmHash.Zero, data).SpanData;
            var hash = bmt.Hash(spanData);
            return new SwarmCac(hash, spanData);
        }
    }
}
