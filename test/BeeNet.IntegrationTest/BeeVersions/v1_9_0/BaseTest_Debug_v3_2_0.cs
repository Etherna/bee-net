﻿using Etherna.BeeNet;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.IntegrationTest.BeeVersions.v1_9_0
{
    public abstract class BaseTest_Debug_V3_2_0
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data/TestFileForUpload_Debug.txt";
        protected const string version = "3.2.0";

        public BaseTest_Debug_V3_2_0()
        {
            beeNodeClient = new BeeNodeClient(
                Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://192.168.1.124/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_2_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v3_2_0);
        }

        protected async Task<string> UploadChunkFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data/TestFileForUpload_Debug.txt");
            await Task.Delay(180000);


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);

            return result.Reference;
        }
    }
}
