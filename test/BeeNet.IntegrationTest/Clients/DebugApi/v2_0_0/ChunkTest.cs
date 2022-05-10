using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v2_0_0
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
        public async Task GetChunkAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.GetChunkAsync("");


            // Assert
        }

        [Fact]
        public async Task DeleteChunkAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.DeleteChunkAsync("");


            // Assert
        }
    }
}