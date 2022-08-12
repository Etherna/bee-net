using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class StewardshipTest : BaseTest_Gateway_v3_0_2
    {
        
        [Fact]
        public async Task CheckIsContentAvailableAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await Task.Delay(90000);

            // Act 
            var result = await beeNodeClient.GatewayClient.CheckIsContentAvailableAsync(reference);


            // Assert 
            Assert.True(result.IsRetrievable);
        }

        [Fact]
        public async Task ReuploadContentAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Act 
            await beeNodeClient.GatewayClient.ReuploadContentAsync(reference);


            // Assert 
        }

    }
}
