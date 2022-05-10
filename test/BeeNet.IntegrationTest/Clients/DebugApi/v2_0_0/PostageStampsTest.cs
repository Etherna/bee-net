using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v2_0_0
{
    public class PostageStampsTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public PostageStampsTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

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


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetPostageBatchAsync("id");


            // Assert
        }

        [Fact]
        public async Task GetStampsBucketsForBatchAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetStampsBucketsForBatchAsync("id");


            // Assert
        }

        [Fact]
        public async Task BuyPostageBatchAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.BuyPostageBatchAsync(5000, 32);


            // Assert
        }

        [Fact]
        public async Task TopUpPostageBatchAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.TopUpPostageBatchAsync("id", 500);


            // Assert
        }

        [Fact]
        public async Task DilutePostageBatchAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.DilutePostageBatchAsync("id", 500);


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
