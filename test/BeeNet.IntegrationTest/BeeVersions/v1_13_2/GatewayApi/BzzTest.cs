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

using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class BzzTest : BaseTest_Gateway_v5_0_0
    {
        
        [Fact]
        public async Task UploadFileSingleFileTextAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);

            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadFileAsync(
                batch,
                content: File.OpenRead("Data/TestFileForUpload_Gateway.txt"),
                name: "TestFileForUpload_Gateway.txt",
                contentType: "text/plain", 
                swarmCollection: false);

            // Assert 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);
            StreamReader reader = new(result.Stream);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }

        [Fact]
        public async Task UploadFileSingleFileTarAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            
            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadFileAsync(
                batch,
                content: File.OpenRead("Data/BzzFIleForUpload.tar"),
                name: "BzzFIleForUpload.tar",
                contentType: "application/x-tar",
                swarmCollection: false);

            // Assert 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);
            StreamReader reader = new(result.Stream);
            Assert.Equal(File.ReadAllText("Data/BzzFIleForUpload.tar"), reader.ReadToEnd());
        }
        /*
        [Fact]
        public async Task UploadFileMultiFileTextAsync()
        {
            // Arrange 
            //var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            //await Task.Delay(180000);
            var batch = "e380bc90bd1a0de1ed674bdf010fac31195fbe1179646d4ded8dd818858c2b32";
            var fileParameterInput = new FileParameterInput("D:\\Etherna\\bee-net\\test\\BeeNet.IntegrationTest\\Data\\TestFileForUpload_Gateway.txt", "TestFileForUpload_Gateway.txt", "text/plain");
            var fileParameterInputSecond = new FileParameterInput("D:\\Etherna\\bee-net\\test\\BeeNet.IntegrationTest\\Data\\TestFileForUpload_GatewaySecond.txt", "TestFileForUpload_GatewaySecond.txt", "text/plain");


            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadFileAsync(batch, file: new List<FileParameterInput> { fileParameterInput, fileParameterInputSecond }, swarmCollection: true);


            // Assert 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);
        }
        */
        [Fact]
        public async Task GetFileAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync(pathTestFileForUpload);


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);


            // Assert 
            StreamReader reader = new(result.Stream);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }

        [Fact]
        public async Task GetFilePathAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync(pathTestFileForUpload);


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileWithPathAsync(reference, "");


            // Assert 
            StreamReader reader = new(result.Stream);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }
        
    }
}
