using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0
{
    public abstract class BaseTest_Debug_v2_0_1
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Debug.txt";
        protected const string version = "2.0.1";

        public BaseTest_Debug_v2_0_1()
        {
            beeNodeClient = new BeeNodeClient(
                System.Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://192.168.1.103/",
                1624,
                1625,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_2,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_1);
        }

        protected async Task<string> UploadChunkFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data\\TestFileForUpload_Debug.txt");
            await Task.Delay(90000);


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);

            return result.Reference;
        }
    }
}
