using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v2_0_0
{
    public class BalanceTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public BalanceTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

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
