using Etherna.BeeNet;
using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi;
using Etherna.BeeNet.InputModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0
{
    public abstract class BaseTest_Gateway_v3_0_2
    {
        protected BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Gateway.txt";
        protected readonly string ethAddress = "0x26234a2ad3ba8b398a762f279b792cfacd536a3f";
        protected readonly string peerId = "03c2d16303362d2ab9e1acc39f4c089cc0669a7a3c11e9e3f30964faca80c6f1";
        protected const string version = "3.0.2";

        public BaseTest_Gateway_v3_0_2()
        {
            beeNodeClient = new BeeNodeClient(
                "http://192.168.1.103/",
                1624,
                1625,
                GatewayApiVersion.v3_0_2,
                DebugApiVersion.v2_0_1);
        }

        public async Task CreateAuthenticatedClientAsync()
        {
            beeNodeClient = await BeeNodeClient.AuthenticatedBeeNodeClientAsync(
                new BeeAuthicationData("admin", "$2a$10$DjfxmdPKiT9eBzSAZ2uiPe2hlaxYfBNBwAKPvi18M/ZZl7ultyDLW"),
                "http://192.168.1.103",
                1624,
                1625,
                GatewayApiVersion.v3_0_2,
                DebugApiVersion.v2_0_1);
        }

        protected async Task<string> UploadBZZFileAndGetReferenceAsync()
        {
            var batch = "855f9389cf57a01369cff81901e4f5191ec03191f9a2d4d806486e9d856f9cdc";//await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            //await Task.Delay(90000);
            var fileParameterInput = new FileParameterInput(File.OpenRead("Data\\BzzFIleForUpload.tar"), "BzzFIleForUpload.tar", "application/x-tar");
            //var fileParameterInput = new FileParameterInput(File.OpenRead("Data\\TestFileForUpload_Gateway.txt"), "TestFileForUpload_Gateway.txt", "text/plain");

            // Act 
            var result = await beeNodeClient.GatewayClient.UploadFileAsync(batch, file: new List<FileParameterInput> { fileParameterInput }, swarmCollection: true);

            return result;
        }

        protected async Task<string> UploadChunkFileAndGetReferenceAsync()
        {
            //var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            var batch = "855f9389cf57a01369cff81901e4f5191ec03191f9a2d4d806486e9d856f9cdc";
            //var tag = await beeNodeClient.GatewayClient.CreateTagAsync(batch);
            var fs = File.OpenRead("Data\\TestFileForUpload_Gateway.txt");
            //await Task.Delay(90000);


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, null, body: fs, swarmDeferredUpload: false);

            return result.Reference;
        }

    }
}
