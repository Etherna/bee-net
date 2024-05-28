//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.BeeNet;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2
{
    public abstract class BaseTest_Debug_v5_0_0
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data/TestFileForUpload_Debug.txt";
        protected const string version = "4.0.0";

        public BaseTest_Debug_v5_0_0()
        {
            beeNodeClient = new BeeNodeClient(
                Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://127.0.0.1/",
                1633);
        }

        protected async Task<string> UploadChunkFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data/TestFileForUpload_Debug.txt");
            await Task.Delay(180000);


            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadChunkAsync(batch, tag.Uid, body: fs);

            return reference;
        }
    }
}
