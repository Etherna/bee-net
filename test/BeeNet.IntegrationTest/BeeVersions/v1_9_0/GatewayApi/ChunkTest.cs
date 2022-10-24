using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_9_0.GatewayApi
{
    public class ChunkTest : BaseTest_Gateway_V3_2_0
    {
        //[Fact]
        //public async Task UploadChunkAsync()
        //{
        //    // Arrange 
        //    var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
        //    var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
        //    var fs = File.OpenRead("Data/TestFileForUpload_Debug.txt");
        //    await Task.Delay(90000);


        //    // Act 
        //    var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);


        //    // Assert
        //    //TODO check stream data
        //}

        //[Fact]
        //public async Task GetChunkAsync()
        //{
        //    // Arrange 
        //    var reference = await UploadChunkFileAndGetReferenceAsync();


        //    // Act 
        //    var result = await beeNodeClient.GatewayClient.GetChunkAsync(reference);


        //    // Assert
        //}

        //[Fact]
        //public async Task ChunksHeadAsync()
        //{
        //    // Arrange 
        //    var reference = await UploadChunkFileAndGetReferenceAsync();


        //    // Act 
        //    var result = await beeNodeClient.GatewayClient.ChunksHeadAsync(reference);


        //    // Assert
        //}

        //[Fact]
        //public async Task DeleteChunkAsync()
        //{
        //    // Arrange 
        //    var reference = await UploadChunkFileAndGetReferenceAsync();


        //    // Act 
        //    var result = await beeNodeClient.GatewayClient.DeleteChunkAsync(reference);


        //    // Assert
        //}
        
    }
}