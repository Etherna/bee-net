using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_9_0.GatewayApi
{
    public class BytesTest : BaseTest_Gateway_V3_2_0
    {
        [Fact]
        public async Task UploadDataAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadDataAsync(batch, body: File.OpenRead(pathTestFileForUpload));


            // Assert
            var result = await beeNodeClient.GatewayClient.GetDataAsync(reference);
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }

        [Fact]
        public async Task GetDataAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var reference = await beeNodeClient.GatewayClient.UploadDataAsync(batch, body: File.OpenRead(pathTestFileForUpload));


            // Act 
            var result = await beeNodeClient.GatewayClient.GetDataAsync(reference);


            // Assert
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }
        
    }
}
