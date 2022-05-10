using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class BytesTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public BytesTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

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
