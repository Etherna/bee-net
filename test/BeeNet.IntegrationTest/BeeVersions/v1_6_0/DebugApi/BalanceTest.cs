using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_0.DebugApi
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
            var address = "5c53c90b5a2f83db4b096c58327f361a63a797e9f12f20e55a6e7ae3e2be92c0";

            // Act 
            var balance = await beeNodeClient.DebugClient.GetBalanceWithPeerAsync(address);


            // Assert
            Assert.Equal(address, balance.Peer);
        }


        [Fact]
        public async Task GetConsumedBalanceWithPeerAsync()
        {
            // Arrange 
            var address = "5c53c90b5a2f83db4b096c58327f361a63a797e9f12f20e55a6e7ae3e2be92c0";


            // Act 
            var balance = await beeNodeClient.DebugClient.GetConsumedBalanceWithPeerAsync("5c53c90b5a2f83db4b096c58327f361a63a797e9f12f20e55a6e7ae3e2be92c0");


            // Assert
            Assert.Equal(address, balance.Peer);
        }
    }
}
