using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_0.GatewayApi
{
    public class PinningTest : BaseTest_Gateway_v3_0_1
    {
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.CreatePinAsync(reference); //TODO Error: The I/O operation has been aborted because of either a thread exit or an application request.


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
