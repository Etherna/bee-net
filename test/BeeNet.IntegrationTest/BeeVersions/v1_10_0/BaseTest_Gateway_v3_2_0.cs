using Etherna.BeeNet;
using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi;
using Etherna.BeeNet.InputModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.IntegrationTest.BeeVersions.v1_10_0
{
    public abstract class BaseTest_Gateway_V3_2_0
    {
        protected BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data/TestFileForUpload_Gateway.txt";
        protected const string version = "3.2.0";

        public BaseTest_Gateway_V3_2_0()
        {
            beeNodeClient = new BeeNodeClient(
                Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://192.168.1.107/",
                1633,
                1635,
                GatewayApiVersion.v4_0_0,
                DebugApiVersion.v4_0_0);
        }

        public async Task CreateAuthenticatedClientAsync()
        {
            beeNodeClient = await BeeNodeClient.AuthenticatedBeeNodeClientAsync(
                Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://192.168.1.107/",
                1633,
                GatewayApiVersion.v4_0_0);
        }

        protected async Task<string> UploadBZZFileAndGetReferenceAsync(string filePath = null)
        {
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            //var fileParameterInput = new FileParameterInput(File.OpenRead("Data/BzzFIleForUpload.tar"), "BzzFIleForUpload.tar", "application/x-tar");
            var fileParameterInput = new FileParameterInput(File.OpenRead(filePath ?? pathTestFileForUpload), Path.GetFileName(filePath) ?? Path.GetFileName(pathTestFileForUpload), "text/plain");

            // Act 
            var result = await beeNodeClient.GatewayClient.UploadFileAsync(batch, files: new List<FileParameterInput> { fileParameterInput }, swarmCollection: false);

            return result;
        }

        protected async Task<string> UploadChunkFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var fs = File.OpenRead(pathTestFileForUpload);


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, null, body: fs, swarmDeferredUpload: false);

            return result.Reference;
        }

    }
}
