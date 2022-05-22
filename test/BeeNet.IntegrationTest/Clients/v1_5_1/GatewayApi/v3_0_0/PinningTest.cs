using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.v1_5_1.GatewayApi.v3_0_0
{
    public class PinningTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.CreatePinAsync("8343797038f5d53ea56daf06a11baa9974585a1f9d2a60360ccf555c064b8a11");


            // Assert 
        }

        [Fact]
        public async Task DeletePinAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.DeletePinAsync("reference");


            // Assert 
        }

        [Fact]
        public async Task GetPinStatusAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetPinStatusAsync("reference");


            // Assert 
        }

        [Fact]
        public async Task GetAllPinsAsync()
        {
            // Arrange 


            // Act 
            var results = await beeNodeClient.GatewayClient.GetAllPinsAsync();


            // Assert 
        }

    }
}
