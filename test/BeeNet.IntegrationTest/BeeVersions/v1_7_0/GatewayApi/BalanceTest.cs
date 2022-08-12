using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class BalanceTest : BaseTest_Gateway_v3_0_2
    {


        [Fact]
        public async Task GetAllBalancesAsync()
        {
            // Arrange 
            await CreateAuthenticatedClientAsync();

            // Act 
            var balances = await beeNodeClient.GatewayClient.GetAllBalancesAsync();


            // Assert
            Assert.Equal(4, balances.Count());
        }

        [Fact]
        public async Task GetAllConsumedBalancesAsync()
        {
            // Arrange 


            // Act 
            var balances = await beeNodeClient.GatewayClient.GetAllConsumedBalancesAsync();


            // Assert
            Assert.Equal(4, balances.Count());
        }

        [Fact]
        public async Task GetBalanceWithPeerAsync()
        {
            // Arrange 


            // Act 
            var balance = await beeNodeClient.GatewayClient.GetBalanceWithPeerAsync(peerId);


            // Assert
            Assert.Equal(peerId, balance.Peer);
        }


        [Fact]
        public async Task GetConsumedBalanceWithPeerAsync()
        {
            // Arrange 


            // Act 
            var balance = await beeNodeClient.GatewayClient.GetConsumedBalanceWithPeerAsync(peerId);


            // Assert
            Assert.Equal(peerId, balance.Peer);
        }

    }
}
