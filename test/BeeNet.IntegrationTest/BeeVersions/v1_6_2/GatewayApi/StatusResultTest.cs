using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_2.GatewayApi
{
    public class StatusResultTest : BaseTest_Gateway_v3_0_2
    {

        [Fact]
        public async Task GetReserveStateAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.GatewayClient.GetReserveStateAsync();


            // Assert
            Assert.True(reserveState.Commitment > 0);
            Assert.True(reserveState.Radius > 0);
        }

        [Fact]
        public async Task GetChainStateAsync()
        {
            // Arrange 


            // Act 
            var chainState = await beeNodeClient.GatewayClient.GetChainStateAsync();


            // Assert
            Assert.True(chainState.Block > 0);
            Assert.True(chainState.ChainTip > 0);
        }

        [Fact]
        public async Task GetNodeInfoAsync()
        {
            // Arrange 


            // Act 
            var nodeInfo = await beeNodeClient.GatewayClient.GetNodeInfoAsync();


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
            var healthAsync = await beeNodeClient.GatewayClient.GetHealthAsync();


            // Assert
            Assert.Equal("3.0.1", healthAsync.ApiVersion);
            Assert.Equal("2.0.1", healthAsync.DebugApiVersion);
            Assert.Equal("ok", healthAsync.Status);
            Assert.StartsWith("1.6.1-", healthAsync.Version);
        }

        [IgnoreOtherVersionFact(testVersion: "1.6.0")]
        public async Task GetHealth2Async()
        {
            // Arrange 


            // Act 
            var healthAsync = await beeNodeClient.GatewayClient.GetHealthAsync();


            // Assert
            Assert.Equal("3.0.1", healthAsync.ApiVersion);
            Assert.Equal("2.0.1", healthAsync.DebugApiVersion);
            Assert.Equal("ok", healthAsync.Status);
            Assert.StartsWith("1.6.1-", healthAsync.Version);
        }


    }
}