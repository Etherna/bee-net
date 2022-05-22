using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.v1_5_1.DebugApi.v2_0_0
{
    public class ChunkTest : BaseTest_Debug_v2_0_0
    {
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