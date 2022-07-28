using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_2.GatewayApi
{
    public class SettlementsTest : BaseTest_Gateway_v3_0_2
    {

        [Fact]
        public async Task GetAllSettlementsAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.GatewayClient.GetAllSettlementsAsync();


            // Assert
            Assert.Contains(reserveState.Settlements, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetAllTimeSettlementsAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.GatewayClient.GetAllTimeSettlementsAsync();


            // Assert
            Assert.Contains(reserveState.Settlements, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetSettlementsWithPeerAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.GatewayClient.GetSettlementsWithPeerAsync(peerId);


            // Assert
            Assert.Equal(peerId, reserveState.Peer);
        }

    }
}
