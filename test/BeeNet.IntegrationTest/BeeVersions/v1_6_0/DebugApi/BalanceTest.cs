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
        }

        [Fact]
        public async Task GetAllConsumedBalancesAsync()
        {
            // Arrange 


            // Act 
            var balances = await beeNodeClient.DebugClient.GetAllConsumedBalancesAsync();


            // Assert
        }

        [Fact]
        public async Task GetBalanceWithPeerAsync()
        {
            // Arrange 


            // Act 
            var balances = await beeNodeClient.DebugClient.GetBalanceWithPeerAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Assert
        }


        [Fact]
        public async Task GetConsumedBalanceWithPeerAsync()
        {
            // Arrange 


            // Act 
            var balances = await beeNodeClient.DebugClient.GetConsumedBalanceWithPeerAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Assert
        }
    }
}
