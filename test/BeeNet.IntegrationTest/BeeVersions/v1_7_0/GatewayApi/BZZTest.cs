using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class BZZTest : BaseTest_Gateway_v3_0_2
    {
        
        [Fact]
        public async Task UploadFileAsync()
        {
            // Arrange 


            // Act 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Assert 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);
        }

        [Fact]
        public async Task GetFileAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);


            // Assert 
            //TODO check if file contains correct data
        }

        [Fact]
        public async Task GetFilePathAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileWithPathAsync(reference, "");


            // Assert 
        }
        
    }
}
