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

using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Chunks
{
    public class ChunkDataStreamTest
    {
        // Internal classes.
        public record JoinChunkDataTestElement(
            IChunkStore ChunkStore,
            byte[] Data,
            SwarmReference RootReference,
            RedundancyStrategy Strategy);
        
        // Data.
        private class JoinChunkDataClassData : IEnumerable<object[]>
        {
            private IEnumerable<object[]> cases = [];

            public JoinChunkDataClassData() =>
                Task.Run(InitializeAsync).GetAwaiter().GetResult();

            private async Task InitializeAsync()
            {
                var tests = new List<JoinChunkDataTestElement>();
                
                var data = new byte[1024 * 1024]; //1MB
                new Random(0).NextBytes(data);
                
                //plain with None strategy
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.None,
                        false,
                        0,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.None));
                }
                
                //plain with Data strategy
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.None,
                        false,
                        0,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //compact level == 65535
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.None,
                        false,
                        65535,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //encrypted
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.None,
                        true,
                        0,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //compact level == 65535 && encrypted
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.None,
                        true,
                        65535,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //redundancy level == Medium (missing root data chunks)
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Medium,
                        false,
                        0,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    await chunkStore.RemoveAsync(hashResult.Hash);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //redundancy level == Medium (missing 3 data chunks)
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Medium,
                        false,
                        0,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                
                    //remove chunks (and verify they are removed)
                    var hashes = new SwarmHash[]
                    {
                        "1bdbaf2c4056f3615fb067f1c781f6767e342bd1dae40e12c8f3d3de69f316cf",
                        "d41ac8c5e6e8fd2848d00e4c0ed01a39d31750f34df4016e4764a8b48a925b64",
                        "1786d0bcfe9c52d57fa39e2f201843e3ef508474fe82cb49a688457db54c4d5c"
                    };
                    foreach (var hash in hashes)
                        Assert.True(await chunkStore.RemoveAsync(hash));
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //redundancy level == Medium (missing 3 data chunks) && compact level == 65535
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Medium,
                        false,
                        65535,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    //remove chunks (and verify they are removed)
                    var hashes = new SwarmHash[]
                    {
                        "b8fafb9d8688fd40f37016ca4eecae1b4a5ad0e1eda646d00e6ca4ba8531e5a3",
                        "ba60b3bf6ad8546b7a1f686f48832b0702e55dc0aec7aeef935efd45c4b83dab",
                        "f54cef97c490651e875190e80b063dd46187dc82e60db1e0733ede2944ac6c91"
                    };
                    foreach (var hash in hashes)
                        Assert.True(await chunkStore.RemoveAsync(hash));
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //redundancy level == Medium (missing 3 random cac chunks) && encrypted
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Medium,
                        true,
                        0,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    //remove random chunks
                    var random = new Random();
                    foreach (var hash in chunkStore.AllChunks.Where(p => p.Value is SwarmCac)
                                 .OrderBy(_ => random.Next())
                                 .Take(3)
                                 .Select(p => p.Key)
                                 .ToArray())
                        await chunkStore.RemoveAsync(hash);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }
                
                //redundancy level == Medium (missing 3 random cac chunks) && compact level == 65535 && encrypted
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Medium,
                        true,
                        0,
                        null);
                    await using var dataStream = new MemoryStream(data);
                    
                    var hashResult = await fileHasherPipeline.HashDataAsync(dataStream);
                    
                    //remove random chunks
                    var random = new Random();
                    foreach (var hash in chunkStore.AllChunks.Where(p => p.Value is SwarmCac)
                                 .OrderBy(_ => random.Next())
                                 .Take(3)
                                 .Select(p => p.Key)
                                 .ToArray())
                        await chunkStore.RemoveAsync(hash);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult,
                        RedundancyStrategy.Data));
                }

                cases = tests.Select(t => new object[] { t });
            }

            public IEnumerator<object[]> GetEnumerator() => cases.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        

        // Tests.
        [Theory, ClassData(typeof(JoinChunkDataClassData))]
        public async Task JoinChunkData(JoinChunkDataTestElement test)
        {
            await using var memoryStream = new MemoryStream();
            await using var chunkDataStream = await ChunkDataStream.BuildNewAsync(
                test.RootReference,
                test.ChunkStore,
                RedundancyLevel.Paranoid,
                test.Strategy,
                true);
            
            await chunkDataStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var result = memoryStream.ToArray();
            
            Assert.Equal(test.Data, result);
        }
    }
}