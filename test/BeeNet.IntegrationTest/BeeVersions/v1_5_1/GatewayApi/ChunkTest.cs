using Etherna.BeeNet;
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


            // Act 
            var result = await beeNodeClient.GatewayClient.GetChunkStreamAsync("1bfca7c27da7fe1758cfaac017b4b0d7b82020ff471aab34b1b23fd9116deed1");


            // Assert
        }

        [Fact]
        public async Task UploadChunkAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32); 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            MemoryStream inMemoryCopy = new MemoryStream();
            using (FileStream fs = File.OpenRead("Data\\TestFileForUpload_Debug.txt"))
            {
                fs.CopyTo(inMemoryCopy);
            }


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: inMemoryCopy);


            // Assert
        }

        [Fact]
        public async Task UploadChunksStreamAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            //var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var stream = System.IO.File.OpenRead("Data\\TestFileForUpload_Debug.txt");

            // Act 
            await beeNodeClient.GatewayClient.UploadChunksStreamAsync(batch);


            // Assert
        }

    }
}
