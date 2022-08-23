using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class FeedTest : BaseTest_Gateway_v3_0_2
    {

        [Fact]
        public async Task CreateFeedAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(90000);
            var addresses = await beeNodeClient.DebugClient.GetAddressesAsync();
            var topic = "cf880b8eeac5093fa27b0825906c600685";


            // Act 
            var result = await beeNodeClient.GatewayClient.CreateFeedAsync(addresses.Ethereum.Replace("0x", ""), topic, batch);


            // Assert 
            Assert.NotEmpty(result);
        }

        /*
        [Fact]
        public async Task GetFeedAsync()
        {
            // Arrange 
            //var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            //await Task.Delay(90000);
            var batch = "855f9389cf57a01369cff81901e4f5191ec03191f9a2d4d806486e9d856f9cdc";
            var addresses = await beeNodeClient.DebugClient.GetAddressesAsync();
            var topic = "cf880b8eeac5093fa27b0825906c600685";
            var feed = await beeNodeClient.GatewayClient.CreateFeedAsync(addresses.Ethereum.Replace("0x", ""), topic, batch);
            await Task.Delay(90000);
            

            // Act 
            var result = await beeNodeClient.GatewayClient.GetFeedAsync(addresses.Ethereum.Replace("0x", ""), topic);


            // Assert 
            Assert.Equal(feed, result);
        }
        */

    }
}
