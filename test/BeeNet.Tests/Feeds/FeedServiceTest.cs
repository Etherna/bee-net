using Etherna.BeeNet.Clients.GatewayApi;
using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Feeds.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Feeds
{
    public class FeedServiceTest
    {
        // Internal classes.
        public class FindStartingEpochOfflineTestElement
        {
            public FindStartingEpochOfflineTestElement(
                ulong at,
                EpochFeedIndex? knownNearEpoch,
                EpochFeedIndex expectedResult)
            {
                At = at;
                KnownNearEpoch = knownNearEpoch;
                ExpectedResult = expectedResult;
            }

            public ulong At { get; }
            public EpochFeedIndex ExpectedResult { get; }
            public EpochFeedIndex? KnownNearEpoch { get; }
        }

        public class TryFindStartingEpochChunkOnlineTestElement
        {
            public TryFindStartingEpochChunkOnlineTestElement(
                byte[] account,
                ulong at,
                EpochFeedIndex epochIndex,
                byte[] topic,
                Action<Mock<IBeeGatewayClient>> arrangeAction,
                Action<Mock<IBeeGatewayClient>> asserts,
                FeedChunk? expectedResult)
            {
                Account = account;
                ArrangeAction = arrangeAction;
                Asserts = asserts;
                At = at;
                EpochIndex = epochIndex;
                ExpectedResult = expectedResult;
                Topic = topic;
            }

            public byte[] Account { get; }
            public Action<Mock<IBeeGatewayClient>> ArrangeAction { get; }
            public Action<Mock<IBeeGatewayClient>> Asserts { get; }
            public ulong At { get; }
            public EpochFeedIndex EpochIndex { get; }
            public FeedChunk? ExpectedResult { get; }
            public byte[] Topic { get; }
        }

        // Fields.
        private readonly FeedService service;
        private readonly Mock<IBeeGatewayClient> gatewayClientMock = new();

        // Constructor.
        public FeedServiceTest()
        {
            service = new FeedService(gatewayClientMock.Object);
        }

        // Data.
        public static IEnumerable<object[]> FindStartingEpochOfflineTests
        {
            get
            {
                var tests = new List<FindStartingEpochOfflineTestElement>
                {
                    // Null known near epoch, date on left epoch at max level.
                    new FindStartingEpochOfflineTestElement(
                        10,
                        null,
                        new EpochFeedIndex(0, 32)),
                    
                    // Null known near epoch, date on right epoch at max level.
                    new FindStartingEpochOfflineTestElement(
                        5_000_000_000,
                        null,
                        new EpochFeedIndex(4_294_967_296, 32)),

                    // Known near epoch prior to date
                    new FindStartingEpochOfflineTestElement(
                        7,
                        new EpochFeedIndex(5, 0),
                        new EpochFeedIndex(4, 2)),

                    // Known near epoch successive to date
                    new FindStartingEpochOfflineTestElement(
                        8,
                        new EpochFeedIndex(14, 1),
                        new EpochFeedIndex(8, 3)),

                    // Known near epoch contains date
                    new FindStartingEpochOfflineTestElement(
                        10,
                        new EpochFeedIndex(8, 2),
                        new EpochFeedIndex(8, 2))
                };

                return tests.Select(t => new object[] { t });
            }
        }

        public static IEnumerable<object[]> TryFindStartingEpochChunkOnlineTests
        {
            get
            {
                var chunkAccount = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
                var chunkTopic = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

                var tests = new List<TryFindStartingEpochChunkOnlineTestElement>();

                // Chunk exists and is prior to date.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 2);
                    var chunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var payload = chunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var reference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, startingEpochIndex);

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        chunkAccount,
                        6,
                        startingEpochIndex,
                        chunkTopic,
                        gateMock => gateMock.Setup(g => g.GetChunkAsync(reference, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new MemoryStream(payload)),
                        gateMock => gateMock.Verify(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once()),
                        new FeedChunk(startingEpochIndex, payload, reference)));
                };

                // Chunk not exists and index was left at max level.
                {
                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        chunkAccount,
                        6,
                        new EpochFeedIndex(0, EpochFeedIndex.MaxLevel),
                        chunkTopic,
                        gateMock => gateMock.Setup(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                            .Throws(() => new BeeNetGatewayApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null)),
                        gateMock => gateMock.Verify(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once()),
                        null));
                }

                // Chunk exists but with successive date and index was left at max level.
                {
                    var startingEpochIndex = new EpochFeedIndex(0, EpochFeedIndex.MaxLevel);
                    var chunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 }; //1970-01-01 00:00:07
                    var payload = chunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var reference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, startingEpochIndex);

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        chunkAccount,
                        6,
                        startingEpochIndex,
                        chunkTopic,
                        gateMock => gateMock.Setup(g => g.GetChunkAsync(reference, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new MemoryStream(payload)),
                        gateMock => gateMock.Verify(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once()),
                        null));
                }

                // Valid chunk is found at left, starting chunk is not found.
                {
                    var startingEpochIndex = new EpochFeedIndex(6, 1);
                    var startingChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, startingEpochIndex);

                    var leftEpochIndex = startingEpochIndex.Left;
                    var leftChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, leftEpochIndex);
                    var leftChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var leftChunkPayload = leftChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        chunkAccount,
                        6,
                        startingEpochIndex,
                        chunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkAsync(startingChunkReference, It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetGatewayApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                            gateMock.Setup(g => g.GetChunkAsync(leftChunkReference, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(leftChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new FeedChunk(leftEpochIndex, leftChunkPayload, leftChunkReference)));
                }

                // Valid chunk is found at left, starting chunk is successive to date.
                {
                    var startingEpochIndex = new EpochFeedIndex(6, 1);
                    var startingChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, startingEpochIndex);
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 }; //1970-01-01 00:00:07
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    var leftEpochIndex = startingEpochIndex.Left;
                    var leftChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, leftEpochIndex);
                    var leftChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var leftChunkPayload = leftChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        chunkAccount,
                        6,
                        startingEpochIndex,
                        chunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkAsync(startingChunkReference, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(startingChunkPayload));
                            gateMock.Setup(g => g.GetChunkAsync(leftChunkReference, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(leftChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new FeedChunk(leftEpochIndex, leftChunkPayload, leftChunkReference)));
                }

                // Chunk valid is found at parent, starting chunk is not found.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 1);
                    var startingChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, startingEpochIndex);

                    var parentEpochIndex = startingEpochIndex.GetParent();
                    var parentChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, parentEpochIndex);
                    var parentChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var parentChunkPayload = parentChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        chunkAccount,
                        6,
                        startingEpochIndex,
                        chunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkAsync(startingChunkReference, It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetGatewayApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                            gateMock.Setup(g => g.GetChunkAsync(parentChunkReference, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(parentChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new FeedChunk(parentEpochIndex, parentChunkPayload, parentChunkReference)));
                }

                // Chunk valid is found at parent, starting chunk is successive to date.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 1);
                    var startingChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, startingEpochIndex);
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 }; //1970-01-01 00:00:07
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    var parentEpochIndex = startingEpochIndex.GetParent();
                    var parentChunkReference = FeedChunk.BuildReferenceHash(chunkAccount, chunkTopic, parentEpochIndex);
                    var parentChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var parentChunkPayload = parentChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        chunkAccount,
                        6,
                        startingEpochIndex,
                        chunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkAsync(startingChunkReference, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(startingChunkPayload));
                            gateMock.Setup(g => g.GetChunkAsync(parentChunkReference, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(parentChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new FeedChunk(parentEpochIndex, parentChunkPayload, parentChunkReference)));
                }

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TryGetFeedChunkAsync(bool chunkExists)
        {
            // Arrange.
            var referenceHash = "aeef03dde6685d5a1c9ae5af374cce84b25aab391222801d8c4dc5d108929592";
            var index = new EpochFeedIndex(2, 1);

            var payload = new byte[] { 0, 0, 0, 1, 2, 3, 4, 5, 6, 7 };
            var payloadStream = new MemoryStream(payload);

            if (chunkExists)
                gatewayClientMock.Setup(c => c.GetChunkAsync(referenceHash, It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<Stream>(payloadStream));
            else
                gatewayClientMock.Setup(c => c.GetChunkAsync(referenceHash, It.IsAny<CancellationToken>()))
                    .Throws(new BeeNetGatewayApiException("", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

            // Act.
            var result = await service.TryGetFeedChunkAsync(referenceHash, index);

            // Assert.
            if (chunkExists)
            {
                Assert.NotNull(result);
                Assert.Equal(index, result.Index);
                Assert.Equal(payload, result.Payload);
                Assert.Equal(referenceHash, result.ReferenceHash);
            }
            else
            {
                Assert.Null(result);
            }
        }

        [Theory, MemberData(nameof(FindStartingEpochOfflineTests))]
        public void FindStartingEpochOffline(FindStartingEpochOfflineTestElement test)
        {
            var result = service.FindStartingEpochOffline(test.KnownNearEpoch, test.At);

            Assert.Equal(test.ExpectedResult, result);
        }

        [Theory, MemberData(nameof(TryFindStartingEpochChunkOnlineTests))]
        public async Task TryFindStartingEpochChunkOnlineAsync(TryFindStartingEpochChunkOnlineTestElement test)
        {
            test.ArrangeAction(gatewayClientMock);

            var result = await service.TryFindStartingEpochChunkOnlineAsync(test.Account, test.Topic, test.At, test.EpochIndex);

            Assert.Equal(test.ExpectedResult, result);
            test.Asserts(gatewayClientMock);
        }
    }
}
