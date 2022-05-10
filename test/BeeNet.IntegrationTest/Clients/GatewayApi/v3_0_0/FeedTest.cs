using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class FeedTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public FeedTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

        [Fact]
        public async Task CreateFeedAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.CreateFeedAsync("owner", "topic", "batchid");


            // Assert 
        }

        [Fact]
        public async Task GetFeedAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.GetFeedAsync("owner", "topic");


            // Assert 
        }

    }
}
