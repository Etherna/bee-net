﻿using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class PostageStampsTest : BaseTest_Gateway_v3_0_2
    {

        [Fact]
        public async Task GetOwnedPostageBatchesByNodeAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.GatewayClient.GetOwnedPostageBatchesByNodeAsync();


            // Assert
        }

        [Fact]
        public async Task GetPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);

            // Act 
            var resultBatch = await beeNodeClient.GatewayClient.GetPostageBatchAsync(batch); //TODO missing data in returned json


            // Assert
            Assert.Equal(500, resultBatch.AmountPaid);
            Assert.Equal(32, resultBatch.Depth);
        }

        [Fact]
        public async Task GetStampsBucketsForBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);


            // Act 
            var reserveState = await beeNodeClient.GatewayClient.GetStampsBucketsForBatchAsync(batch); //TODO swarm address


            // Assert
        }

        [Fact]
        public async Task BuyPostageBatchAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);


            // Assert
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

        [Fact]
        public async Task DilutePostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.GatewayClient.DilutePostageBatchAsync(batch, 64);


            // Assert
            Assert.Equal(batch, result);
        }

        [Fact]
        public async Task GetAllValidPostageBatchesFromAllNodesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetAllValidPostageBatchesFromAllNodesAsync();


            // Assert
        }

    }
}
