using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.DebugApi
{
    public class ChunkTest : BaseTest_Debug_v3_0_2
    {
        /*
        [Fact]
        public async Task GetChunkAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.DebugClient.GetChunkAsync(reference); //TODO address


            // Assert
        }

        [Fact]
        public async Task DeleteChunkAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.DebugClient.DeleteChunkAsync(reference);


            // Assert
        }

        private async Task<string> UploadFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data/TestFileForUpload_Debug.txt");
            await Task.Delay(90000);


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);

            return result.Reference;
        }

        */
    }
}