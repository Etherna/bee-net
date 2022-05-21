using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class PinningTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.CreatePinAsync("a0da79fda8d8e117af98e0f4fe6665dccb2b5ec3b862ad6cf7d20065d03f3a21");


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
