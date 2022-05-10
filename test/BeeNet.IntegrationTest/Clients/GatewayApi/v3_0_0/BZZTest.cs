using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class BZZTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public BZZTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

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
            var result = await beeNodeClient.GatewayClient.GetFileAsync("reference", "path", null);


            // Assert 
        }

    }
}
