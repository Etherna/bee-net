using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class PinningTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public PinningTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.CreatePinAsync("reference");


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
