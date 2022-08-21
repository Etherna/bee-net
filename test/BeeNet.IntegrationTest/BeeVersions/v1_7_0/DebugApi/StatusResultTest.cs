using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.DebugApi
{
    public class StatusResultTest : BaseTest_Debug_v3_0_2
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
            Assert.False(nodeInfo.GatewayMode);
            Assert.True(nodeInfo.SwapEnabled);
        }

        [IgnoreOtherVersionFact(testVersion: version)]
        public async Task GetHealthAsync()
        {
            // Arrange 


            // Act 
            var healthAsync = await beeNodeClient.DebugClient.GetHealthAsync();


            // Assert
            Assert.Equal("3.0.1", healthAsync.ApiVersion);
            Assert.Equal("2.0.1", healthAsync.DebugApiVersion);
            Assert.Equal("ok", healthAsync.Status);
            Assert.StartsWith("1.7.0-", healthAsync.Version);
        }
    }
}