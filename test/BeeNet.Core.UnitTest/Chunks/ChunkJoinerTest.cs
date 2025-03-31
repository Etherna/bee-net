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
using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Chunks
{
    public class ChunkJoinerTest
    {
        // Internal classes.
        public class JoinChunkDataTestElement(
            IChunkStore chunkStore,
            byte[] data,
            SwarmChunkReference rootChunkReference)
        {
            public IChunkStore ChunkStore { get; } = chunkStore;
            public byte[] Data { get; } = data;
            public SwarmChunkReference RootChunkReference { get; } = rootChunkReference;
        }
        
        // Data.
        public static IEnumerable<object[]> JoinChunkDataTests
        {
            get
            {
                var tests = new List<JoinChunkDataTestElement>();
                
                var data = new byte[1024 * 1024]; //1MB
                new Random(0).NextBytes(data);
                
                //use compact level == 0
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        () => new Hasher(),
                        new FakePostageStamper(),
                        RedundancyLevel.None,
                        false,
                        0,
                        null);
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
                }
                
                //use compact level == 65535
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        () => new Hasher(),
                        new FakePostageStamper(),
                        RedundancyLevel.None,
                        false,
                        65535,
                        null);
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
                }

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        [Theory, MemberData(nameof(JoinChunkDataTests))]
        public async Task JoinChunkData(JoinChunkDataTestElement test)
        {
            using var memoryStream = new MemoryStream();
            var chunkJoiner = new ChunkJoiner(test.ChunkStore);
            
            using var resultStream = await chunkJoiner.GetJoinedChunkDataAsync(test.RootChunkReference);
            await resultStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var result = memoryStream.ToArray();
            
            Assert.Equal(test.Data, result);
        }
    }
}