using Etherna.BeeNet.InputModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_12_0.GatewayApi
{
    public class BzzTest : BaseTest_Gateway_v4_0_0
    {
        
        [Fact]
        public async Task UploadFileSingleFileTextAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var fileParameterInput = new FileParameterInput(File.OpenRead("Data/TestFileForUpload_Gateway.txt"), "TestFileForUpload_Gateway.txt", "text/plain");


            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadFileAsync(batch, files: new List<FileParameterInput> { fileParameterInput }, swarmCollection: false);


            // Assert 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }

        [Fact]
        public async Task UploadFileSingleFileTarAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.DebugClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var fileParameterInput = new FileParameterInput(File.OpenRead("Data/BzzFIleForUpload.tar"), "BzzFIleForUpload.tar", "application/x-tar");


            // Act 
            var reference = await beeNodeClient.GatewayClient.UploadFileAsync(batch, files: new List<FileParameterInput> { fileParameterInput }, swarmCollection: false);


            // Assert 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);
            StreamReader reader = new(result);
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
            StreamReader reader = new(result);
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
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }
        
    }
}
