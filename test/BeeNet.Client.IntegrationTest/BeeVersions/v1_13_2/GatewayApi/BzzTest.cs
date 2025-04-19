// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.Client.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class BzzTest : BaseTest_Gateway_v5_0_0
    {
        
        [Fact]
        public async Task UploadFileSingleFileTextAsync()
        {
            // Arrange 
            var (batch, _) = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);

            // Act 
            var reference = await beeNodeClient.UploadFileAsync(
                batch,
                content: File.OpenRead("Data/TestFileForUpload_Gateway.txt"),
                name: "TestFileForUpload_Gateway.txt",
                contentType: "text/plain", 
                isFileCollection: false);

            // Assert 
            var result = await beeNodeClient.GetFileAsync(reference);
            StreamReader reader = new(result.Stream);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }

        [Fact]
        public async Task UploadFileSingleFileTarAsync()
        {
            // Arrange 
            var (batch, _) = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            
            // Act 
            var reference = await beeNodeClient.UploadFileAsync(
                batch,
                content: File.OpenRead("Data/BzzFIleForUpload.tar"),
                name: "BzzFIleForUpload.tar",
                contentType: "application/x-tar",
                isFileCollection: false);

            // Assert 
            var result = await beeNodeClient.GetFileAsync(reference);
            StreamReader reader = new(result.Stream);
            Assert.Equal(File.ReadAllText("Data/BzzFIleForUpload.tar"), reader.ReadToEnd());
        }
        /*
        [Fact]
        public async Task UploadFileMultiFileTextAsync()
        {
            // Arrange 
            //var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            //await Task.Delay(180000);
            var batch = "e380bc90bd1a0de1ed674bdf010fac31195fbe1179646d4ded8dd818858c2b32";
            var fileParameterInput = new FileParameterInput("D:\\Etherna\\bee-net\\test\\BeeNet.IntegrationTest\\Data\\TestFileForUpload_Gateway.txt", "TestFileForUpload_Gateway.txt", "text/plain");
            var fileParameterInputSecond = new FileParameterInput("D:\\Etherna\\bee-net\\test\\BeeNet.IntegrationTest\\Data\\TestFileForUpload_GatewaySecond.txt", "TestFileForUpload_GatewaySecond.txt", "text/plain");


            // Act 
            var reference = await beeNodeClient.UploadFileAsync(batch, file: new List<FileParameterInput> { fileParameterInput, fileParameterInputSecond }, swarmCollection: true);


            // Assert 
            var result = await beeNodeClient.GetFileAsync(reference);
        }
        */
        [Fact]
        public async Task GetFileAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync(pathTestFileForUpload);


            // Act 
            var result = await beeNodeClient.GetFileAsync(reference);


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
            var result = await beeNodeClient.GetFileAsync(reference);


            // Assert 
            StreamReader reader = new(result.Stream);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }
        
    }
}
