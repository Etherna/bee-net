using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.v1_5_1.GatewayApi.v3_0_0
{
    public class BZZTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task UploadFileAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadFileAsync("batchId");


            // Assert 
        }

        [Fact]
        public async Task GetFileAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileAsync("reference");


            // Assert 
        }

        [Fact]
        public async Task GetFilePathAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileAsync("reference");


            // Assert 
        }

    }
}
