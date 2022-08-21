using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_5_1.GatewayApi
{
    public class PostalServiceSwarmTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task SendPssAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.SendPssAsync("topic", "targets", "batchId");


            // Assert 
        }

        [Fact]
        public async Task SubscribeToPssAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.SubscribeToPssAsync("topic");


            // Assert 
        }
    }
}
