using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_2.GatewayApi
{
    public class StewardshipTest : BaseTest_Gateway_v3_0_2
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

        [Fact]
        public async Task ReuploadContentAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            await beeNodeClient.GatewayClient.ReuploadContentAsync(reference);


            // Assert 
        }

    }
}
