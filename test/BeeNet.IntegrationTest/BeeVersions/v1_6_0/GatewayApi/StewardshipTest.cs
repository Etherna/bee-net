using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_0.GatewayApi
{
    public class StewardshipTest : BaseTest_Gateway_v3_0_1
    {
        [Fact]
        public async Task CheckIsContentAvailableAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            await beeNodeClient.GatewayClient.CheckIsContentAvailableAsync(reference);
        

            // Assert 
        }
    }
}
