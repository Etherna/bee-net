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

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.Client.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class PostageStampsTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetOwnedPostageBatchesByNodeAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var postageBatches = await beeNodeClient.GetOwnedPostageBatchesByNodeAsync();


            // Assert
            Assert.Contains(postageBatches, i => i.Id == batch);
        }

        [Fact]
        public async Task GetPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            

            // Act 
            var resultBatch = await beeNodeClient.GetPostageBatchAsync(batch);


            // Assert
            Assert.Equal(500, resultBatch.Amount);
            Assert.Equal(32, resultBatch.Depth);
        }

        [Fact]
        public async Task GetPostageBatchBucketsAsync()
        {
            // Arrange.
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);

            // Act.
            var reserveState = await beeNodeClient.GetPostageBatchBucketsAsync(batch);

            // Assert.
            Assert.Equal(32, reserveState.Depth);
        }

        [Fact]
        public async Task BuyPostageBatchAsync()
        {
            // Act.
            var result = await beeNodeClient.BuyPostageBatchAsync(500, 32);

            // Assert.
            await Task.Delay(60000);
            var batch = await beeNodeClient.GetPostageBatchAsync(result);
            Assert.Equal(batch.Id, result);
        }

        [Fact]
        public async Task TopUpPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.TopUpPostageBatchAsync(batch, 64);


            // Assert
            Assert.Equal(batch, result);
        }

        //TODO Invalid depth
        /*
        [Fact]
        public async Task DilutePostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 62);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.DilutePostageBatchAsync(batch, 32);


            // Assert
            Assert.Equal(batch, result);
        }*/

        [Fact]
        public async Task GetAllValidPostageBatchesFromAllNodesAsync()
        {
            // Arrange 
            var batchId = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            
            // Act 
            var results = await beeNodeClient.GetAllValidPostageBatchesFromAllNodesAsync();

            // Assert
            Assert.Contains(results, i => i.Value.Any(b => b.Id == batchId));
        }
    }
}
