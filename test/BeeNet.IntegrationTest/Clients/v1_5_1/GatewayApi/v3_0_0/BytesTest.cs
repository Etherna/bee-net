using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.v1_5_1.GatewayApi.v3_0_0
{
    public class BytesTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task UploadDataAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadDataAsync("");


            // Assert
        }

        [Fact]
        public async Task GetDataAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetDataAsync("");


            // Assert
        }

    }
}
