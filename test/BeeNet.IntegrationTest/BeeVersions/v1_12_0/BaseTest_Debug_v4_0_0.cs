using Etherna.BeeNet;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.IntegrationTest.BeeVersions.v1_12_0
{
    public abstract class BaseTest_Debug_v4_0_0
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data/TestFileForUpload_Debug.txt";
        protected const string version = "4.0.0";

        public BaseTest_Debug_v4_0_0()
        {
            beeNodeClient = new BeeNodeClient(
                Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://192.168.1.110/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v4_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v4_0_0);
        }

        protected async Task<string> UploadChunkFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data/TestFileForUpload_Debug.txt");
            await Task.Delay(180000);


            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);

            return reference;
        }
    }
}
