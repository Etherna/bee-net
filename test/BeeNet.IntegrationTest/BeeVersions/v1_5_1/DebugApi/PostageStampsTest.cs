﻿using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_5_1.DebugApi
{
    public class PostageStampsTest : BaseTest_Debug_v2_0_0
    {
        [Fact]
        public async Task GetOwnedPostageBatchesByNodeAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetOwnedPostageBatchesByNodeAsync();


            // Assert
        }

        [Fact]
        public async Task GetPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);

            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetPostageBatchAsync(batch); //TODO missing data in returned json


            // Assert
        }

        [Fact]
        public async Task GetStampsBucketsForBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetStampsBucketsForBatchAsync(batch); //TODO swarm address


            // Assert
        }

        [Fact]
        public async Task BuyPostageBatchAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);


            // Assert
        }

        [Fact]
        public async Task TopUpPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.DebugClient.TopUpPostageBatchAsync(batch, 64);


            // Assert
        }

        [Fact]
        public async Task DilutePostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.DebugClient.DilutePostageBatchAsync(batch, 64);


            // Assert
        }

        [Fact]
        public async Task GetAllValidPostageBatchesFromAllNodesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.GetAllValidPostageBatchesFromAllNodesAsync();


            // Assert
        }

    }
}