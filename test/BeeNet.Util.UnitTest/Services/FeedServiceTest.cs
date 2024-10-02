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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Models.Feeds;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Services
{
    public class FeedServiceTest
    {
        // Internal classes.
        public class FindLastEpochChunkBeforeDateTestElement
        {
            public FindLastEpochChunkBeforeDateTestElement(
                byte[] account,
                ulong at,
                SwarmFeedChunk startingChunk,
                byte[] topic,
                Action<Mock<IBeeClient>> arrangeAction,
                Action<Mock<IBeeClient>> asserts,
                SwarmFeedChunk expectedResult)
            {
                Account = account;
                ArrangeAction = arrangeAction;
                Asserts = asserts;
                At = at;
                ExpectedResult = expectedResult;
                StartingChunk = startingChunk;
                Topic = topic;
            }

            public byte[] Account { get; }
            public Action<Mock<IBeeClient>> ArrangeAction { get; }
            public Action<Mock<IBeeClient>> Asserts { get; }
            public ulong At { get; }
            public SwarmFeedChunk ExpectedResult { get; }
            public SwarmFeedChunk StartingChunk { get; }
            public byte[] Topic { get; }
        }

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
                Action<Mock<IBeeClient>> arrangeAction,
                Action<Mock<IBeeClient>> asserts,
                SwarmFeedChunk? expectedResult)
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
            public Action<Mock<IBeeClient>> ArrangeAction { get; }
            public Action<Mock<IBeeClient>> Asserts { get; }
            public ulong At { get; }
            public EpochFeedIndex EpochIndex { get; }
            public SwarmFeedChunk? ExpectedResult { get; }
            public byte[] Topic { get; }
        }

        // Consts.
        private static readonly byte[] ChunkAccount = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        private static readonly byte[] ChunkTopic = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

        // Fields.
        private readonly Mock<IBeeClient> gatewayClientMock = new();
        private readonly FeedService service;

        // Constructor.
        public FeedServiceTest()
        {
            service = new FeedService(gatewayClientMock.Object);
        }

        // Data.
        public static IEnumerable<object[]> FindLastEpochChunkBeforeDateTests
        {
            get
            {
                var tests = new List<FindLastEpochChunkBeforeDateTestElement>();

                // Starting chunk epoch index is at max resolution (level 0).
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 0);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var startingChunk = new SwarmFeedChunk(startingEpochIndex, startingChunkPayload, startingChunkReference);

                    tests.Add(new FindLastEpochChunkBeforeDateTestElement(
                        ChunkAccount,
                        6,
                        startingChunk,
                        ChunkTopic,
                        _ => { },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Never()),
                        startingChunk));
                }

                // Chunk with child epoch at date is valid and at max resolution.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 1);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 4 }; //1970-01-01 00:00:04
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var startingChunk = new SwarmFeedChunk(startingEpochIndex, startingChunkPayload, startingChunkReference);

                    var childEpochIndex = new EpochFeedIndex(5, 0);
                    var childChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, childEpochIndex, new Hasher());
                    var childChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var childChunkPayload = childChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var childChunk = new SwarmFeedChunk(childEpochIndex, childChunkPayload, childChunkReference);

                    tests.Add(new FindLastEpochChunkBeforeDateTestElement(
                        ChunkAccount,
                        6,
                        startingChunk,
                        ChunkTopic,
                        gateMock => gateMock.Setup(g => g.GetChunkStreamAsync(
                                childChunkReference,
                                It.IsAny<int>(),
                                It.IsAny<bool?>(),
                                It.IsAny<long?>(),
                                It.IsAny<string?>(),
                                It.IsAny<string?>(),
                                It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new MemoryStream(childChunkPayload)),
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Once()),
                        childChunk));
                }

                // Chunk with left brother of child epoch at date is valid and at max resolution.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 1);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 4 }; //1970-01-01 00:00:04
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var startingChunk = new SwarmFeedChunk(startingEpochIndex, startingChunkPayload, startingChunkReference);

                    var rightChildEpochIndex = new EpochFeedIndex(5, 0);
                    var rightChildChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, rightChildEpochIndex, new Hasher());

                    var leftChildEpochIndex = new EpochFeedIndex(4, 0);
                    var leftChildChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, leftChildEpochIndex, new Hasher());
                    var leftChildChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 4 }; //1970-01-01 00:00:04
                    var leftChildChunkPayload = leftChildChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var leftChildChunk = new SwarmFeedChunk(leftChildEpochIndex, leftChildChunkPayload, leftChildChunkReference);

                    tests.Add(new FindLastEpochChunkBeforeDateTestElement(
                        ChunkAccount,
                        5,
                        startingChunk,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    rightChildChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    leftChildChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(leftChildChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        leftChildChunk));
                }

                // Chunk on child at date epoch is left and doesn't exist.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 2);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 4 }; //1970-01-01 00:00:04
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var startingChunk = new SwarmFeedChunk(startingEpochIndex, startingChunkPayload, startingChunkReference);

                    var leftChildEpochIndex = new EpochFeedIndex(4, 1);
                    var leftChildChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, leftChildEpochIndex, new Hasher());

                    tests.Add(new FindLastEpochChunkBeforeDateTestElement(
                        ChunkAccount,
                        5,
                        startingChunk,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    leftChildChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Once()),
                        startingChunk));
                }

                // Chunks on child at date and its left brother epochs don't exist.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 2);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 4 }; //1970-01-01 00:00:04
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var startingChunk = new SwarmFeedChunk(startingEpochIndex, startingChunkPayload, startingChunkReference);

                    var rightChildEpochIndex = new EpochFeedIndex(6, 1);
                    var rightChildChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, rightChildEpochIndex, new Hasher());

                    var leftChildEpochIndex = new EpochFeedIndex(4, 1);
                    var leftChildChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, leftChildEpochIndex, new Hasher());

                    tests.Add(new FindLastEpochChunkBeforeDateTestElement(
                        ChunkAccount,
                        6,
                        startingChunk,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    rightChildChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    leftChildChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        startingChunk));
                }

                // Chunk on child at date (right) is successive to date, and chunk on its left brother epoch doesn't exist.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 2);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 4 }; //1970-01-01 00:00:04
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var startingChunk = new SwarmFeedChunk(startingEpochIndex, startingChunkPayload, startingChunkReference);

                    var leftChildEpochIndex = new EpochFeedIndex(4, 1);
                    var leftChildChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, leftChildEpochIndex, new Hasher());

                    var rightChildEpochIndex = new EpochFeedIndex(6, 1);
                    var rightChildChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, rightChildEpochIndex, new Hasher());
                    var rightChildChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 }; //1970-01-01 00:00:07
                    var rightChildChunkPayload = rightChildChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var rightChildChunk = new SwarmFeedChunk(rightChildEpochIndex, rightChildChunkPayload, rightChildChunkReference);

                    tests.Add(new FindLastEpochChunkBeforeDateTestElement(
                        ChunkAccount,
                        6,
                        startingChunk,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    rightChildChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(rightChildChunkPayload));
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    leftChildChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        startingChunk));
                }

                return tests.Select(t => new object[] { t });
            }
        }

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

                    // Known near epoch prior to date.
                    new FindStartingEpochOfflineTestElement(
                        7,
                        new EpochFeedIndex(5, 0),
                        new EpochFeedIndex(4, 2)),

                    // Known near epoch successive to date.
                    new FindStartingEpochOfflineTestElement(
                        8,
                        new EpochFeedIndex(14, 1),
                        new EpochFeedIndex(8, 3)),

                    // Known near epoch contains date.
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
                var tests = new List<TryFindStartingEpochChunkOnlineTestElement>();

                // Chunk exists and is prior to date.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 2);
                    var chunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var payload = chunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var reference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        ChunkAccount,
                        6,
                        startingEpochIndex,
                        ChunkTopic,
                        gateMock => gateMock.Setup(g => g.GetChunkStreamAsync(
                                reference,
                                It.IsAny<int>(),
                                It.IsAny<bool?>(),
                                It.IsAny<long?>(),
                                It.IsAny<string?>(),
                                It.IsAny<string?>(),
                                It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new MemoryStream(payload)),
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Once()),
                        new SwarmFeedChunk(startingEpochIndex, payload, reference)));
                };

                // Chunk not exists and index was left at max level.
                {
                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        ChunkAccount,
                        6,
                        new EpochFeedIndex(0, EpochFeedIndex.MaxLevel),
                        ChunkTopic,
                        gateMock => gateMock.Setup(g => g.GetChunkStreamAsync(
                                It.IsAny<SwarmHash>(),
                                It.IsAny<int>(),
                                It.IsAny<bool?>(),
                                It.IsAny<long?>(),
                                It.IsAny<string?>(),
                                It.IsAny<string?>(),
                                It.IsAny<CancellationToken>()))
                            .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null)),
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Once()),
                        null));
                }

                // Chunk exists but with successive date and index was left at max level.
                {
                    var startingEpochIndex = new EpochFeedIndex(0, EpochFeedIndex.MaxLevel);
                    var chunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 }; //1970-01-01 00:00:07
                    var payload = chunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload
                    var reference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        ChunkAccount,
                        6,
                        startingEpochIndex,
                        ChunkTopic,
                        gateMock => gateMock.Setup(g => g.GetChunkStreamAsync(
                                reference,
                                It.IsAny<int>(),
                                It.IsAny<bool?>(),
                                It.IsAny<long?>(),
                                It.IsAny<string?>(),
                                It.IsAny<string?>(),
                                It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new MemoryStream(payload)),
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Once()),
                        null));
                }

                // Valid chunk is found at left, starting chunk is not found.
                {
                    var startingEpochIndex = new EpochFeedIndex(6, 1);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());

                    var leftEpochIndex = startingEpochIndex.Left;
                    var leftChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, leftEpochIndex, new Hasher());
                    var leftChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var leftChunkPayload = leftChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        ChunkAccount,
                        6,
                        startingEpochIndex,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    startingChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    leftChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(leftChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new SwarmFeedChunk(leftEpochIndex, leftChunkPayload, leftChunkReference)));
                }

                // Valid chunk is found at left, starting chunk is successive to date.
                {
                    var startingEpochIndex = new EpochFeedIndex(6, 1);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 }; //1970-01-01 00:00:07
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    var leftEpochIndex = startingEpochIndex.Left;
                    var leftChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, leftEpochIndex, new Hasher());
                    var leftChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var leftChunkPayload = leftChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        ChunkAccount,
                        6,
                        startingEpochIndex,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    startingChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(startingChunkPayload));
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    leftChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(leftChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new SwarmFeedChunk(leftEpochIndex, leftChunkPayload, leftChunkReference)));
                }

                // Chunk valid is found at parent, starting chunk is not found.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 1);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());

                    var parentEpochIndex = startingEpochIndex.Parent;
                    var parentChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, parentEpochIndex, new Hasher());
                    var parentChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var parentChunkPayload = parentChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        ChunkAccount,
                        6,
                        startingEpochIndex,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    startingChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .Throws(() => new BeeNetApiException("not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    parentChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(parentChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new SwarmFeedChunk(parentEpochIndex, parentChunkPayload, parentChunkReference)));
                }

                // Chunk valid is found at parent, starting chunk is successive to date.
                {
                    var startingEpochIndex = new EpochFeedIndex(4, 1);
                    var startingChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, startingEpochIndex, new Hasher());
                    var startingChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 }; //1970-01-01 00:00:07
                    var startingChunkPayload = startingChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    var parentEpochIndex = startingEpochIndex.Parent;
                    var parentChunkReference = SwarmFeedChunk.BuildHash(ChunkAccount, ChunkTopic, parentEpochIndex, new Hasher());
                    var parentChunkTimestamp = new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }; //1970-01-01 00:00:05
                    var parentChunkPayload = parentChunkTimestamp.Concat(new byte[] { 1, 2, 3 }).ToArray(); //arbitrary content payload

                    tests.Add(new TryFindStartingEpochChunkOnlineTestElement(
                        ChunkAccount,
                        6,
                        startingEpochIndex,
                        ChunkTopic,
                        gateMock =>
                        {
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    startingChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(startingChunkPayload));
                            gateMock.Setup(g => g.GetChunkStreamAsync(
                                    parentChunkReference,
                                    It.IsAny<int>(),
                                    It.IsAny<bool?>(),
                                    It.IsAny<long?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<string?>(),
                                    It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new MemoryStream(parentChunkPayload));
                        },
                        gateMock => gateMock.Verify(g => g.GetChunkStreamAsync(
                            It.IsAny<SwarmHash>(),
                            It.IsAny<int>(),
                            It.IsAny<bool?>(),
                            It.IsAny<long?>(),
                            It.IsAny<string?>(),
                            It.IsAny<string?>(),
                            It.IsAny<CancellationToken>()), Times.Exactly(2)),
                        new SwarmFeedChunk(parentEpochIndex, parentChunkPayload, parentChunkReference)));
                }

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        [Theory, MemberData(nameof(FindLastEpochChunkBeforeDateTests))]
        public async Task FindLastEpochChunkBeforeDate(FindLastEpochChunkBeforeDateTestElement test)
        {
            test.ArrangeAction(gatewayClientMock);

            var result = await service.FindLastEpochChunkBeforeDateAsync(test.Account, test.Topic, test.At, test.StartingChunk);

            Assert.Equal(test.ExpectedResult, result);
            test.Asserts(gatewayClientMock);
        }

        [Theory, MemberData(nameof(FindStartingEpochOfflineTests))]
        public void FindStartingEpochOffline(FindStartingEpochOfflineTestElement test)
        {
            var result = FeedService.FindStartingEpochOffline(test.KnownNearEpoch, test.At);

            Assert.Equal(test.ExpectedResult, result);
        }

        [Theory, MemberData(nameof(TryFindStartingEpochChunkOnlineTests))]
        public async Task TryFindStartingEpochChunkOnline(TryFindStartingEpochChunkOnlineTestElement test)
        {
            test.ArrangeAction(gatewayClientMock);

            var result = await service.TryFindStartingEpochChunkOnlineAsync(test.Account, test.Topic, test.At, test.EpochIndex);

            Assert.Equal(test.ExpectedResult, result);
            test.Asserts(gatewayClientMock);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TryGetFeedChunkAsync(bool chunkExists)
        {
            // Arrange.
            var referenceHash = "aeef03dde6685d5a1c9ae5af374cce84b25aab391222801d8c4dc5d108929592";
            var index = new EpochFeedIndex(2, 1);

            var data = new byte[] { 0, 0, 0, 1, 2, 3, 4, 5, 6, 7 };
            var dataStream = new MemoryStream(data);

            if (chunkExists)
                gatewayClientMock.Setup(c => c.GetChunkStreamAsync(
                        referenceHash,
                        It.IsAny<int>(),
                        It.IsAny<bool?>(),
                        It.IsAny<long?>(),
                        It.IsAny<string?>(),
                        It.IsAny<string?>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<Stream>(dataStream));
            else
                gatewayClientMock.Setup(c => c.GetChunkStreamAsync(
                        referenceHash,
                        It.IsAny<int>(),
                        It.IsAny<bool?>(),
                        It.IsAny<long?>(),
                        It.IsAny<string?>(),
                        It.IsAny<string?>(),
                        It.IsAny<CancellationToken>()))
                    .Throws(new BeeNetApiException("", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

            // Act.
            var result = await service.TryGetFeedChunkAsync(referenceHash, index);

            // Assert.
            if (chunkExists)
            {
                Assert.NotNull(result);
                Assert.Equal(index, result.Index);
                Assert.Equal(data, result.Data);
                Assert.Equal(referenceHash, result.Hash);
            }
            else
            {
                Assert.Null(result);
            }
        }
    }
}
