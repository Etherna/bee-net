using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_0
{
    public abstract class BaseTest_Gateway_v3_0_1
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Gateway.txt";
        protected readonly string ethAddress = "0x26234a2ad3ba8b398a762f279b792cfacd536a3f";

        public BaseTest_Gateway_v3_0_1()
        {
            beeNodeClient = new BeeNodeClient(
                "http://192.168.1.103/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }


        protected async Task<string> UploadFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data\\TestFileForUpload_Gateway.txt");
            await Task.Delay(90000);


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);

            return result.Reference;
        }
    }
}
