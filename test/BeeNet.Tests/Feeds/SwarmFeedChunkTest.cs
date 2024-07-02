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
using Etherna.BeeNet.Hasher;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Models.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Etherna.BeeNet.Feeds
{
    public class SwarmFeedChunkTest
    {
        // Internal classes.
        public class VerifyConstructorArgumentsTestElement
        {
            public VerifyConstructorArgumentsTestElement(
                FeedIndexBase index,
                byte[] payload,
                SwarmHash referenceHash,
                Type expectedExceptionType)
            {
                ExpectedExceptionType = expectedExceptionType;
                Index = index;
                Payload = payload;
                ReferenceHash = referenceHash;
            }

            public Type ExpectedExceptionType { get; }
            public FeedIndexBase Index { get; }
            public byte[] Payload { get; }
            public SwarmHash ReferenceHash { get; }
        }

        // Data.
        public static IEnumerable<object[]> VerifyConstructorArgumentsTests
        {
            get
            {
                var tests = new List<VerifyConstructorArgumentsTestElement>
                {
                    // Shorter payload.
                    new VerifyConstructorArgumentsTestElement(
                        new EpochFeedIndex(0, 0),
                        new byte[SwarmFeedChunk.MinDataSize - 1],
                        "aeef03dde6685d5a1c9ae5af374cce84b25aab391222801d8c4dc5d108929592",
                        typeof(ArgumentOutOfRangeException)),

                    // Longer payload.
                    new VerifyConstructorArgumentsTestElement(
                        new EpochFeedIndex(0, 0),
                        new byte[SwarmChunk.DataSize + 1],
                        "aeef03dde6685d5a1c9ae5af374cce84b25aab391222801d8c4dc5d108929592",
                        typeof(ArgumentOutOfRangeException))
                };

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        [Theory, MemberData(nameof(VerifyConstructorArgumentsTests))]
        public void VerifyConstructorArguments(VerifyConstructorArgumentsTestElement test)
        {
            Assert.Throws(test.ExpectedExceptionType,
                () => new SwarmFeedChunk(test.Index!, test.Payload!, test.ReferenceHash!));
        }

        [Fact]
        public void GetContentPayload()
        {
            var chunk = new SwarmFeedChunk(
                new EpochFeedIndex(0, 0),
                new byte[] { 0, 0, 0, 1, 2, 3, 4, 5, 6, 7 },
                "aeef03dde6685d5a1c9ae5af374cce84b25aab391222801d8c4dc5d108929592");

            var result = chunk.Payload;

            Assert.Equal(new byte[] { 6, 7 }, result);
        }

        [Fact]
        public void GetTimeStamp()
        {
            var chunk = new SwarmFeedChunk(
                new EpochFeedIndex(0, 0),
                new byte[] { 0, 0, 0, 1, 2, 3, 4, 5, 6, 7 },
                "aeef03dde6685d5a1c9ae5af374cce84b25aab391222801d8c4dc5d108929592");

            var result = chunk.TimeStamp;

            Assert.Equal(new DateTimeOffset(2107, 03, 04, 22, 02, 45, TimeSpan.Zero), result);
        }

        [Fact]
        public void BuildChunkPayloadVerifyContentPayload()
        {
            var contentPayload = new byte[SwarmFeedChunk.MaxPayloadSize + 1];

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SwarmFeedChunk.BuildChunkPayload(contentPayload));
        }

        [Fact]
        public void BuildChunkPayload()
        {
            var contentPayload = new byte[] { 4, 2, 0 };

            var beforeTimeStamp = DateTimeOffset.UtcNow;
            Thread.Sleep(1000);
            var chunkPayload = SwarmFeedChunk.BuildChunkPayload(contentPayload);
            Thread.Sleep(1000);
            var afterTimeStamp = DateTimeOffset.UtcNow;

            var chunkUnixTimeStamp = chunkPayload.Take(SwarmFeedChunk.TimeStampSize).ToArray().ByteArrayToUnixDateTime();
            var chunkTimeStamp = DateTimeOffset.FromUnixTimeSeconds((long)chunkUnixTimeStamp);

            Assert.InRange(chunkTimeStamp, beforeTimeStamp, afterTimeStamp);
            Assert.Equal(contentPayload, chunkPayload.Skip(SwarmFeedChunk.TimeStampSize));
        }

        [Fact]
        public void BuildIdentifierVerifyTopic()
        {
            var topic = new byte[] { 1, 2, 3 };
            var index = new EpochFeedIndex(0, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SwarmFeedChunk.BuildIdentifier(topic, index, new HashProvider()));
        }

        [Fact]
        public void BuildIdentifier()
        {
            var topic = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
            var index = new EpochFeedIndex(2, 1);

            var result = SwarmFeedChunk.BuildIdentifier(topic, index, new HashProvider());

            Assert.Equal(
                new byte[] { 229, 116, 252, 141, 32, 73, 147, 48, 181, 92, 124, 96, 74, 217, 20, 163, 90, 16, 124, 66, 174, 221, 76, 184, 135, 58, 193, 210, 235, 104, 138, 215 },
                result);
        }

        [Fact]
        public void BuildHashVerifyAccount()
        {
            var account = new byte[] { 0, 1, 2, 3 };
            var identifier = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SwarmFeedChunk.BuildHash(account, identifier, new HashProvider()));
        }

        [Fact]
        public void BuildHashVerifyIdentifier()
        {
            var account = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            var identifier = new byte[] { 0, 1, 2, 3 };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SwarmFeedChunk.BuildHash(account, identifier, new HashProvider()));
        }

        [Fact]
        public void BuildHash()
        {
            var account = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            var identifier = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

            var result = SwarmFeedChunk.BuildHash(account, identifier, new HashProvider());

            Assert.Equal(
                "854f1dd0c708a544e282b25b9f9c1d353dca28e352656993ab3c2c17b384a86f",
                result);
        }
    }
}
