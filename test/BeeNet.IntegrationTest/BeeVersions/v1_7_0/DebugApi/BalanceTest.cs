using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.DebugApi
{
    public class BalanceTest : BaseTest_Debug_v2_0_1
    {


        [Fact]
        public async Task GetAllBalancesAsync()
        {
            // Arrange 


            // Act 
            var balances = await beeNodeClient.DebugClient.GetAllBalancesAsync();


            // Assert
            Assert.Equal(4, balances.Count());
        }

        [Fact]
        public async Task GetAllConsumedBalancesAsync()
        {
            // Arrange 


            // Act 
            var balances = await beeNodeClient.DebugClient.GetAllConsumedBalancesAsync();


            // Assert
            Assert.Equal(4, balances.Count());
        }

        [Fact]
        public async Task GetBalanceWithPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var balance = await beeNodeClient.DebugClient.GetBalanceWithPeerAsync(peerId);


            // Assert
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
