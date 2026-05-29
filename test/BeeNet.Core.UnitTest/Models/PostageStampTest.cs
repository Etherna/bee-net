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
using Nethereum.Signer;
using System;
using System.Linq;
using Xunit;

namespace Etherna.BeeNet.Models
{
    public class PostageStampTest
    {
        private const string PrivateKeyHex = "0x4f3edf983ac636a65a842ce7c78d9aa706d3b113bce9c46f30d7d21715b23b1d";

        private static PostageBatchId BatchId =>
            Enumerable.Range(0, PostageBatchId.BatchIdSize).Select(i => (byte)(i + 1)).ToArray();
        private static SwarmHash Hash =>
            Enumerable.Range(0, SwarmHash.HashSize).Select(i => (byte)(i + 100)).ToArray();
        private static PostageBucketIndex BucketIndex => new(5, 9);
        private static DateTimeOffset TimeStamp => DateTimeOffset.FromUnixTimeMilliseconds(1_700_000_000_000);
        private static byte[] Signature =>
            Enumerable.Range(0, 65).Select(i => (byte)(i + 7)).ToArray();

        // Tests.
        [Fact]
        public void RoundTripsThroughByteArray()
        {
            var stamp = new PostageStamp(BatchId, BucketIndex, TimeStamp, Signature);

            var bytes = stamp.ToByteArray();
            var rebuilt = PostageStamp.FromByteArray(bytes);

            Assert.Equal(PostageStamp.StampSize, bytes.Length);
            Assert.Equal(stamp.ToByteArray(), rebuilt.ToByteArray());
            Assert.Equal(stamp.BatchId, rebuilt.BatchId);
            Assert.Equal(stamp.BucketIndex.BucketId, rebuilt.BucketIndex.BucketId);
            Assert.Equal(stamp.BucketIndex.BucketCounter, rebuilt.BucketIndex.BucketCounter);
            Assert.Equal(stamp.TimeStamp, rebuilt.TimeStamp);
            Assert.Equal(stamp.Signature.ToArray(), rebuilt.Signature.ToArray());
        }

        [Fact]
        public void RoundTripsThroughString()
        {
            var stamp = new PostageStamp(BatchId, BucketIndex, TimeStamp, Signature);

            var rebuilt = PostageStamp.FromString(stamp.ToString());

            Assert.Equal(stamp.ToByteArray(), rebuilt.ToByteArray());
        }

        [Fact]
        public void ParseAndTryParseRoundTrip()
        {
            var stamp = new PostageStamp(BatchId, BucketIndex, TimeStamp, Signature);
            var asString = stamp.ToString();

            var parsed = PostageStamp.Parse(asString, null);
            Assert.Equal(stamp.ToByteArray(), parsed.ToByteArray());

            Assert.True(PostageStamp.TryParse(asString, null, out var tryParsed));
            Assert.Equal(stamp.ToByteArray(), tryParsed.ToByteArray());

            Assert.False(PostageStamp.TryParse(null, null, out _));
            Assert.False(PostageStamp.TryParse("   ", null, out _));
        }

        // RecoverBatchOwner must recover the signer from a raw 65-byte r||s||v signature
        // (the Swarm/Bee signature layout).
        [Fact]
        public void RecoverBatchOwnerReturnsSigner()
        {
            var signer = new PrivateKeySigner(new EthECKey(PrivateKeyHex));
            var hash = Hash;
            var digest = PostageStamp.BuildSignDigest(hash, BatchId, BucketIndex, TimeStamp, new Hasher());
            var signature = signer.Sign(digest);
            var stamp = new PostageStamp(BatchId, BucketIndex, TimeStamp, signature);

            var recovered = stamp.RecoverBatchOwner(hash, new Hasher());

            Assert.Equal(signer.PublicAddress, recovered);
        }
    }
}
