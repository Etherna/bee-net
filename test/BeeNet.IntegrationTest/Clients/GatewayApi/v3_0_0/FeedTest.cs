using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class FeedTest : BaseTest_Gateway_v3_0_0
    {
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
