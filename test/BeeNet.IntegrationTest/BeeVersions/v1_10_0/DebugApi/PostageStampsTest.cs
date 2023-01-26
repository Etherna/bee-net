using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_10_0.DebugApi
{
    public class PostageStampsTest : BaseTest_Debug_V3_2_0
    {

        [Fact]
        public async Task GetOwnedPostageBatchesByNodeAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var postageBatches = await beeNodeClient.DebugClient.GetOwnedPostageBatchesByNodeAsync();


            // Assert
            Assert.Contains(postageBatches, i => i.Id == batch);
        }

        [Fact]
        public async Task GetPostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            

            // Act 
            var resultBatch = await beeNodeClient.DebugClient.GetPostageBatchAsync(batch);


            // Assert
            Assert.Equal(500, resultBatch.AmountPaid);
            Assert.Equal(32, resultBatch.Depth);
        }

        [Fact]
        public async Task GetStampsBucketsForBatchAsync()
        {
            // Arrange.
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);

            // Act.
            var reserveState = await beeNodeClient.DebugClient.GetStampsBucketsForBatchAsync(batch);

            // Assert.
            Assert.Equal(32, reserveState.Depth);
        }

        [Fact]
        public async Task BuyPostageBatchAsync()
        {
            // Act.
            var result = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);

            // Assert.
            await Task.Delay(60000);
            var batch = await beeNodeClient.DebugClient.GetPostageBatchAsync(result);
            Assert.Equal(batch.Id, result);
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
            Assert.Equal(batch, result);
        }

        //TODO Invalid depth
        /*
        [Fact]
        public async Task DilutePostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 62);
            await Task.Delay(60000);

            // Act 
            var result = await beeNodeClient.DebugClient.DilutePostageBatchAsync(batch, 32);


            // Assert
            Assert.Equal(batch, result);
        }*/

        [Fact]
        public async Task GetAllValidPostageBatchesFromAllNodesAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var results = await beeNodeClient.DebugClient.GetAllValidPostageBatchesFromAllNodesAsync();


            // Assert
            Assert.Contains(results, i => i.BatchID == batch);
        }

    }
}
