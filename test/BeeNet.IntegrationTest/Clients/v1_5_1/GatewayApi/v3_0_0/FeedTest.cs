using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.v1_5_1.GatewayApi.v3_0_0
{
    public class FeedTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task CreateFeedAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(60000);


            // Act 
            await beeNodeClient.GatewayClient.CreateFeedAsync(ethAddress, "cf880b8eeac5093fa27b0825906c600685", batch);


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
