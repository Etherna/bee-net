using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v2_0_0
{
    public class SettlementsTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public SettlementsTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

        [Fact]
        public async Task GetAllSettlementsAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetAllSettlementsAsync();


            // Assert
        }

        [Fact]
        public async Task GetAllTimeSettlementsAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetAllTimeSettlementsAsync();


            // Assert
        }

        [Fact]
        public async Task GetSettlementsWithPeerAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetSettlementsWithPeerAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Assert
        }

    }
}
