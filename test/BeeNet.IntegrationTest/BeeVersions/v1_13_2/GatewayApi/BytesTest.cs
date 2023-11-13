﻿//   Copyright 2021-present Etherna SA
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

using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class BytesTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task UploadDataAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadDataAsync(swarmPostageBatchId: batch, body: File.OpenRead(pathTestFileForUpload));


            // Assert
            var result = await beeNodeClient.GatewayClient.GetDataAsync(reference);
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }

        [Fact]
        public async Task GetDataAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var reference = await beeNodeClient.GatewayClient.UploadDataAsync(swarmPostageBatchId: batch, body: File.OpenRead(pathTestFileForUpload));


            // Act 
            var result = await beeNodeClient.GatewayClient.GetDataAsync(reference);


            // Assert
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }
        
    }
}