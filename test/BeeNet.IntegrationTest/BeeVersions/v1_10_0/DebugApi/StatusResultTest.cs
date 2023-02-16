using Etherna.BeeNet.DtoModels;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_10_0.DebugApi
{
    public class StatusResultTest : BaseTest_Debug_V3_2_0
    {

        [Fact]
        public async Task GetReserveStateAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetReserveStateAsync();


            // Assert
            Assert.True(reserveState.Commitment > 0);
            Assert.True(reserveState.Radius > 0);
        }

        [Fact]
        public async Task GetChainStateAsync()
        {
            // Arrange 


            // Act 
            var chainState = await beeNodeClient.DebugClient.GetChainStateAsync();


            // Assert
            Assert.True(chainState.Block > 0);
        }

        [Fact]
        public async Task GetNodeInfoAsync()
        {
            // Arrange 


            // Act 
            var nodeInfo = await beeNodeClient.DebugClient.GetNodeInfoAsync();


            // Assert
            Assert.True(nodeInfo.ChequebookEnabled);
            Assert.True(nodeInfo.SwapEnabled);
        }

        [IgnoreOtherVersionFact(testVersion: version)]
        public async Task GetHealthAsync()
        {
            // Act 
            var healthAsync = await beeNodeClient.DebugClient.GetHealthAsync();

            // Assert
            Assert.Equal("3.0.2", healthAsync.ApiVersion);
            Assert.Equal("3.0.2", healthAsync.DebugApiVersion);
            Assert.Equal(StatusEnumDto.Ok, healthAsync.Status);
            Assert.StartsWith("1.7.0-", healthAsync.Version);
        }
    }
}