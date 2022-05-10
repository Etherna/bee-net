using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class ChunkTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public ChunkTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

        [Fact]
        public async Task GetChunkStreamAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetChunkStreamAsync("");


            // Assert
        }

        [Fact]
        public async Task UploadChunkAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync("");


            // Assert
        }

        [Fact]
        public async Task UploadChunksStreamAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.UploadChunksStreamAsync("");


            // Assert
        }

    }
}
