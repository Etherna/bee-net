using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v2_0_0
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
            var reserveState = await beeNodeClient.DebugClient.GetPostageBatchAsync(batch);


            // Assert
        }

        [Fact]
        public async Task GetStampsBucketsForBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);

            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetStampsBucketsForBatchAsync(batch);


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

            // Act 
            var result = await beeNodeClient.DebugClient.TopUpPostageBatchAsync(batch, 50);


            // Assert
        }

        [Fact]
        public async Task DilutePostageBatchAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);


            // Act 
            var result = await beeNodeClient.DebugClient.DilutePostageBatchAsync(batch, 32);


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
