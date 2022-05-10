using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class SingleOwnerChunkTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public SingleOwnerChunkTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

        [Fact]
        public async Task SendPssAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.UploadSocAsync("owner", "id", "sig");


            // Assert 
        }
    }
}
