using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_5_1.DebugApi
{
    public class StatusResultTest : BaseTest_Debug_v2_0_0
    {
        [Fact]
        public async Task GetReserveStateAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetReserveStateAsync();


            // Assert
        }

        [Fact]
        public async Task GetChainStateAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetChainStateAsync();


            // Assert
        }

        [Fact]
        public async Task GetNodeInfoAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetNodeInfoAsync();


            // Assert
        }

        [Fact]
        public async Task GetHealthAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetHealthAsync();


            // Assert
        }

        [Fact]
        public async Task GetReadinessAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetReadinessAsync();


            // Assert
        }

    }
}