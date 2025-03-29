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
using Etherna.BeeNet.Stores;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Range = Moq.Range;

namespace Etherna.BeeNet.Models
{
    public class SwarmSequenceFeedTest
    {
        // Internal classes.
        public class LookupSequenceFeedTestElement(
            SwarmSequenceFeedIndex? knownNearIndex,
            Action<Mock<IReadOnlyChunkStore>> setupChunkStore,
            SwarmFeedChunk? expectedResult,
            ulong[] expectedIndexLookups,
            ulong[] expectedOptionalIndexLookups)
        {
            public SwarmSequenceFeedIndex? KnownNearIndex { get; } = knownNearIndex;
            public Action<Mock<IReadOnlyChunkStore>> SetupChunkStore { get; } = setupChunkStore;
            public ulong[] ExpectedIndexLookups { get; } = expectedIndexLookups;
            public ulong[] ExpectedOptionalIndexLookups { get; } = expectedOptionalIndexLookups;
            public SwarmFeedChunk? ExpectedResult { get; } = expectedResult;
        }
        
        // Consts.
        private static readonly EthAddress ChunkOwner = EthAddress.FromByteArray([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19]);
        private static readonly byte[] ChunkTopic = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];

        // Fields.
        private readonly Mock<IReadOnlyChunkStore> chunkStoreMock = new();

        // Data.
        public static IEnumerable<object[]> LookupSequenceFeedTests
        {
            get
            {
                var tests = new List<LookupSequenceFeedTestElement>
                {
                    // Simple lookup without known near index.
                    new(knownNearIndex: null,
                        setupChunkStore: chunkStoreMock =>
                        {
                            for (ulong i = 0; i <= 10; i++)
                            {
                                var chunk = BuildSequenceFeedChunk(i);
                                chunkStoreMock.Setup(c => c.TryGetAsync(
                                        chunk.Hash,
                                        It.IsAny<bool>(),
                                        It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(chunk);
                            }
                        },
                        expectedIndexLookups: [0, 1, 3, 7, 15, 8, 10, 14, 11, 13, 31, 63, 127, 255],
                        expectedOptionalIndexLookups: [],
                        expectedResult: BuildSequenceFeedChunk(10)),
                    
                    // Simple lookup with known near index.
                    new(knownNearIndex: new SwarmSequenceFeedIndex(5),
                        setupChunkStore: chunkStoreMock =>
                        {
                            for (ulong i = 0; i <= 10; i++)
                            {
                                var chunk = BuildSequenceFeedChunk(i);
                                chunkStoreMock.Setup(c => c.TryGetAsync(
                                        chunk.Hash,
                                        It.IsAny<bool>(),
                                        It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(chunk);
                            }
                        },
                        expectedIndexLookups: [5, 6, 8, 12, 9, 11, 10, 20, 36, 68, 132, 260],
                        expectedOptionalIndexLookups: [],
                        expectedResult: BuildSequenceFeedChunk(10)),
                    
                    // Simple lookup with known near index that points to last chunk.
                    new(knownNearIndex: new SwarmSequenceFeedIndex(10),
                        setupChunkStore: chunkStoreMock =>
                        {
                            for (ulong i = 0; i <= 10; i++)
                            {
                                var chunk = BuildSequenceFeedChunk(i);
                                chunkStoreMock.Setup(c => c.TryGetAsync(
                                        chunk.Hash,
                                        It.IsAny<bool>(),
                                        It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(chunk);
                            }
                        },
                        expectedIndexLookups: [10, 11, 13, 17, 25, 41, 73, 137, 265],
                        expectedOptionalIndexLookups: [],
                        expectedResult: BuildSequenceFeedChunk(10)),
                    
                    // Simple lookup with known near index that points to not existing chunk.
                    new(knownNearIndex: new SwarmSequenceFeedIndex(15),
                        setupChunkStore: chunkStoreMock =>
                        {
                            for (ulong i = 0; i <= 10; i++)
                            {
                                var chunk = BuildSequenceFeedChunk(i);
                                chunkStoreMock.Setup(c => c.TryGetAsync(
                                        chunk.Hash,
                                        It.IsAny<bool>(),
                                        It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(chunk);
                            }
                        },
                        expectedIndexLookups: [15],
                        expectedOptionalIndexLookups: [],
                        expectedResult: null),
                    
                    // Lookup on a feed with a long sequence of chunks (> 255).
                    new(knownNearIndex: null,
                        setupChunkStore: chunkStoreMock =>
                        {
                            for (ulong i = 0; i <= 300; i++)
                            {
                                var chunk = BuildSequenceFeedChunk(i);
                                chunkStoreMock.Setup(c => c.TryGetAsync(
                                        chunk.Hash,
                                        It.IsAny<bool>(),
                                        It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(chunk);
                            }
                        },
                        expectedIndexLookups: [0, 1, 3, 7, 15, 31, 63, 127, 255, 256, 258, 262, 270, 286, 318, 287, 289, 293, 301, 294, 296, 300, 317, 382, 510],
                        expectedOptionalIndexLookups: [],
                        expectedResult: BuildSequenceFeedChunk(300)),
                    
                    // Lookup with a false negative chunk's get.
                    new(knownNearIndex: null,
                        setupChunkStore: chunkStoreMock =>
                        {
                            for (ulong i = 0; i <= 10; i++)
                            {
                                if (i == 3)
                                    continue;
                            
                                var chunk = BuildSequenceFeedChunk(i);
                                chunkStoreMock.Setup(c => c.TryGetAsync(
                                        chunk.Hash,
                                        It.IsAny<bool>(),
                                        It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(chunk);
                            }
                        },
                        expectedIndexLookups: [0, 1, 3, 7, 15, 8, 10, 14, 11, 13, 31, 63, 127, 255],
                        expectedOptionalIndexLookups: [2],
                        expectedResult: BuildSequenceFeedChunk(10))
                };

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        
        [Theory, MemberData(nameof(LookupSequenceFeedTests))]
        public async Task LookupSequenceFeed(LookupSequenceFeedTestElement test)
        {
            // Setup.
            var hasher = new Hasher();
            var sequenceFeed = new SwarmSequenceFeed(ChunkOwner, ChunkTopic);
            test.SetupChunkStore(chunkStoreMock);

            // Act.
            var result = await sequenceFeed.TryFindFeedAtAsync(
                chunkStoreMock.Object,
                test.KnownNearIndex,
                requestsCustomTimeout: null);

            // Assert.
            Assert.Equal(test.ExpectedResult?.Hash, result?.Hash);
                        
            chunkStoreMock.Verify(cs => cs.TryGetAsync(
                    It.IsAny<SwarmHash>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()),
                Times.Between(
                    test.ExpectedIndexLookups.Length,
                    test.ExpectedIndexLookups.Length + test.ExpectedOptionalIndexLookups.Length,
                    Range.Inclusive));
            
            foreach (var index in test.ExpectedIndexLookups)
            {
                var hash = SwarmFeedBase.BuildHash(ChunkOwner, ChunkTopic,
                    new SwarmSequenceFeedIndex(index), hasher);
                chunkStoreMock.Verify(cs => cs.TryGetAsync(
                        hash,
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            }
            
            foreach (var index in test.ExpectedOptionalIndexLookups)
            {
                var hash = SwarmFeedBase.BuildHash(ChunkOwner, ChunkTopic,
                    new SwarmSequenceFeedIndex(index), hasher);
                chunkStoreMock.Verify(cs => cs.TryGetAsync(
                        hash,
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>()),
                    Times.AtMostOnce);
            }
        }
        
        // Helpers.
        private static SwarmFeedChunk BuildSequenceFeedChunk(ulong i)
        {
            var index = new SwarmSequenceFeedIndex(i);
            var hash = SwarmFeedBase.BuildHash(ChunkOwner, ChunkTopic, index, new Hasher());
            var data = BitConverter.GetBytes(i);
            return new SwarmFeedChunk(index, data, hash);
        }
    }
}