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
            SwarmReference RootReference);
        
        // Data.
        public static IEnumerable<object[]> JoinChunkDataTests
        {
            get
            {
                var tests = new List<JoinChunkDataTestElement>();
                
                var data = new byte[1024 * 1024]; //1MB
                new Random(0).NextBytes(data);
                
                //plain
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
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
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
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
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
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
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
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
                    
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    chunkStore.RemoveAsync(hashResult.Hash).Wait();
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
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
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                
                    //remove chunks (and verify they are removed)
                    var whenAllTasks = Task.WhenAll(
                        chunkStore.RemoveAsync("1bdbaf2c4056f3615fb067f1c781f6767e342bd1dae40e12c8f3d3de69f316cf"),
                        chunkStore.RemoveAsync("d41ac8c5e6e8fd2848d00e4c0ed01a39d31750f34df4016e4764a8b48a925b64"),
                        chunkStore.RemoveAsync("1786d0bcfe9c52d57fa39e2f201843e3ef508474fe82cb49a688457db54c4d5c"));
                    whenAllTasks.Wait();
                    foreach (var result in whenAllTasks.Result)
                        Assert.True(result);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
                }
                
                //redundancy level == Medium (missing 3 data chunks) && compact level == 65535
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Strong,
                        false,
                        65535,
                        null);
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    //remove chunks (and verify they are removed)
                    var whenAllTasks = Task.WhenAll(
                        chunkStore.RemoveAsync("b8fafb9d8688fd40f37016ca4eecae1b4a5ad0e1eda646d00e6ca4ba8531e5a3"),
                        chunkStore.RemoveAsync("ba60b3bf6ad8546b7a1f686f48832b0702e55dc0aec7aeef935efd45c4b83dab"),
                        chunkStore.RemoveAsync("f54cef97c490651e875190e80b063dd46187dc82e60db1e0733ede2944ac6c91"));
                    whenAllTasks.Wait();
                    foreach (var result in whenAllTasks.Result)
                        Assert.True(result);
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
                }
                
                //redundancy level == Medium (missing 3 random cac chunks) && encrypted
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Strong,
                        true,
                        0,
                        null);
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    //remove random chunks
                    var random = new Random();
                    foreach (var hash in chunkStore.AllChunks.Where(p => p.Value is SwarmCac)
                                 .OrderBy(_ => random.Next())
                                 .Take(3)
                                 .Select(p => p.Key)
                                 .ToArray())
                        chunkStore.RemoveAsync(hash).Wait();
                    
                    tests.Add(new(
                        chunkStore,
                        data,
                        hashResult));
                }
                
                //redundancy level == Medium (missing 3 random cac chunks) && compact level == 65535 && encrypted
                {
                    var chunkStore = new MemoryChunkStore();
                    using var fileHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                        chunkStore,
                        new FakePostageStamper(),
                        RedundancyLevel.Strong,
                        true,
                        0,
                        null);
                    using var dataStream = new MemoryStream(data);
                    
                    var task = fileHasherPipeline.HashDataAsync(dataStream);
                    task.Wait();
                    var hashResult = task.Result;
                    
                    //remove random chunks
                    var random = new Random();
                    foreach (var hash in chunkStore.AllChunks.Where(p => p.Value is SwarmCac)
                                 .OrderBy(_ => random.Next())
                                 .Take(3)
                                 .Select(p => p.Key)
                                 .ToArray())
                        chunkStore.RemoveAsync(hash).Wait();
                    
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
            await using var memoryStream = new MemoryStream();
            await using var chunkDataStream = await ChunkDataStream.BuildNewAsync(
                test.RootReference,
                test.ChunkStore,
                RedundancyLevel.Paranoid,
                RedundancyStrategy.Data,
                false);
            
            await chunkDataStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var result = memoryStream.ToArray();
            
            Assert.Equal(test.Data, result);
        }
    }
}