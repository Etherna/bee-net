using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_12_0.DebugApi
{
    public class BalanceTest : BaseTest_Debug_v4_0_0
    {


        [Fact]
        public async Task GetAllBalancesAsync()
        {
            // Act.
            await beeNodeClient.DebugClient.GetAllBalancesAsync();
        }

        [Fact]
        public async Task GetAllConsumedBalancesAsync()
        {
            // Act.
            await beeNodeClient.DebugClient.GetAllConsumedBalancesAsync();
        }

        [Fact]
        public async Task GetBalanceWithPeerAsync()
        {
            // Arrange .
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();

            // Act.
            var balance = await beeNodeClient.DebugClient.GetBalanceWithPeerAsync(peerId);

            // Assert.
            Assert.Equal(peerId, balance.Peer);
        }


        [Fact]
        public async Task GetConsumedBalanceWithPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var balance = await beeNodeClient.DebugClient.GetConsumedBalanceWithPeerAsync(peerId);


            // Assert
            Assert.Equal(peerId, balance.Peer);
        }

    }
}
