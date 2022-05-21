using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.GatewayApi.v3_0_0
{
    public class SingleOwnerChunkTest : BaseTest_Gateway_v3_0_0
    {
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
