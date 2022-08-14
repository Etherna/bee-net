using Etherna.BeeNet;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class PinningTest : BaseTest_Gateway_v3_0_2
    {
        
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.CreatePinAsync(reference);


            // Assert 
            Assert.True(result.Code == 200 || result.Code == 201);
        }

        [Fact]
        public async Task DeletePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.GatewayClient.CreatePinAsync(reference);


            // Act 
            var result = await beeNodeClient.GatewayClient.DeletePinAsync(reference);


            // Assert 
            Assert.Equal(200, result.Code);
        }

        [Fact]
        public async Task GetPinStatusAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.GatewayClient.CreatePinAsync(reference);
            await Task.Delay(60000);


            // Act 
            var result = await beeNodeClient.GatewayClient.GetPinStatusAsync(reference);


            // Assert 
        }

        [Fact]
        public async Task GetAllPinsAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            var pinCreated = await beeNodeClient.GatewayClient.CreatePinAsync(reference);


            // Act 
            var results = await beeNodeClient.GatewayClient.GetAllPinsAsync();


            // Assert 
            Assert.NotEmpty(results);
        }
        
    }
}
