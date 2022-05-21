using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class StewardshipTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task CheckIsContentAvailableAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.CheckIsContentAvailableAsync("reference");
        

            // Assert 
        }
    }
}
