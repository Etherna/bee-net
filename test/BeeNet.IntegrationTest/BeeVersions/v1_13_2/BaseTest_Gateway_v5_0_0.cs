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
    public abstract class BaseTest_Gateway_v5_0_0
    {
        protected BeeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data/TestFileForUpload_Gateway.txt";
        protected const string version = "4.0.0";

        public BaseTest_Gateway_v5_0_0()
        {
            beeNodeClient = new BeeClient(
                Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://127.0.0.1/",
                1633);
        }

        protected async Task<string> UploadBZZFileAndGetReferenceAsync(string filePath = null)
        {
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);

            // Act 
            var result = await beeNodeClient.UploadFileAsync(
                batch,
                content: File.OpenRead(filePath ?? pathTestFileForUpload),
                name: Path.GetFileName(filePath) ?? Path.GetFileName(pathTestFileForUpload),
                contentType: "text/plain",
                swarmCollection: false);

            return result;
        }

        protected async Task<string> UploadChunkFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var fs = File.OpenRead(pathTestFileForUpload);


            // Act 
            var reference = await beeNodeClient.UploadChunkAsync(batch, null, body: fs, swarmDeferredUpload: false);

            return reference;
        }

    }
}
