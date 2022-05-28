using Etherna.BeeNet;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_5_1.GatewayApi
{
    public class ChunkTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task GetChunkStreamAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();

            // Act 
            var result = await beeNodeClient.GatewayClient.GetChunkStreamAsync(reference);


            // Assert
            var reader = new StreamReader(result);
            var text = reader.ReadToEnd();
            Assert.Contains("File for upload in integration test Gateway", text);
        }

        [Fact]
        public async Task UploadChunkAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32); 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data\\TestFileForUpload_Gateway.txt");
            await Task.Delay(90000);


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);


            // Assert
        }

        [Fact]
        public async Task UploadChunksStreamAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            //var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var stream = System.IO.File.OpenRead("Data\\TestFileForUpload_Gateway.txt");

            // Act 
            await beeNodeClient.GatewayClient.UploadChunksStreamAsync(batch);


            // Assert
        }
    }
}
