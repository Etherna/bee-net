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

using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class PostageStampsTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetOwnedPostageBatchesByNodeAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var postageBatches = await beeNodeClient.GatewayClient.GetOwnedPostageBatchesByNodeAsync();


            // Assert
            Assert.Contains(postageBatches, i => i.Id == batch);
        }

        [Fact]
        public async Task GetPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            

            // Act 
            var resultBatch = await beeNodeClient.GatewayClient.GetPostageBatchAsync(batch);


            // Assert
            Assert.Equal(500, resultBatch.AmountPaid);
            Assert.Equal(32, resultBatch.Depth);
        }

        [Fact]
        public async Task GetStampsBucketsForBatchAsync()
        {
            // Arrange.
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);

            // Act.
            var reserveState = await beeNodeClient.GatewayClient.GetStampsBucketsForBatchAsync(batch);

            // Assert.
            Assert.Equal(32, reserveState.Depth);
        }

        [Fact]
        public async Task BuyPostageBatchAsync()
        {
            // Act.
            var result = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);

            // Assert.
            await Task.Delay(60000);
            var batch = await beeNodeClient.GatewayClient.GetPostageBatchAsync(result);
            Assert.Equal(batch.Id, result);
        }

        [Fact]
        public async Task TopUpPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.GatewayClient.TopUpPostageBatchAsync(batch, 64);


            // Assert
            Assert.Equal(batch, result);
        }

        //TODO Invalid depth
        /*
        [Fact]
        public async Task DilutePostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 62);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.GatewayClient.DilutePostageBatchAsync(batch, 32);


            // Assert
            Assert.Equal(batch, result);
        }*/

        [Fact]
        public async Task GetAllValidPostageBatchesFromAllNodesAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var results = await beeNodeClient.GatewayClient.GetAllValidPostageBatchesFromAllNodesAsync();


            // Assert
            Assert.Contains(results, i => i.BatchID == batch);
        }
    }
}
