using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_5_1.GatewayApi
{
    public class PinningTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.CreatePinAsync(reference);


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
