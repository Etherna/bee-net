//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Models;
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
                SwarmAddress referenceHash,
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
            public SwarmAddress ReferenceHash { get; }
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
                SwarmFeedChunk.BuildIdentifier(topic, index));
        }

        [Fact]
        public void BuildIdentifier()
        {
            var topic = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
            var index = new EpochFeedIndex(2, 1);

            var result = SwarmFeedChunk.BuildIdentifier(topic, index);

            Assert.Equal(
                new byte[] { 229, 116, 252, 141, 32, 73, 147, 48, 181, 92, 124, 96, 74, 217, 20, 163, 90, 16, 124, 66, 174, 221, 76, 184, 135, 58, 193, 210, 235, 104, 138, 215 },
                result);
        }

        [Fact]
        public void BuildReferenceHashVerifyAccount()
        {
            var account = new byte[] { 0, 1, 2, 3 };
            var identifier = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SwarmFeedChunk.BuildReferenceHash(account, identifier));
        }

        [Fact]
        public void BuildReferenceHashVerifyIdentifier()
        {
            var account = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            var identifier = new byte[] { 0, 1, 2, 3 };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SwarmFeedChunk.BuildReferenceHash(account, identifier));
        }

        [Fact]
        public void BuildReferenceHash()
        {
            var account = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            var identifier = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

            var result = SwarmFeedChunk.BuildReferenceHash(account, identifier);

            Assert.Equal(
                "854f1dd0c708a544e282b25b9f9c1d353dca28e352656993ab3c2c17b384a86f",
                result);
        }
    }
}
