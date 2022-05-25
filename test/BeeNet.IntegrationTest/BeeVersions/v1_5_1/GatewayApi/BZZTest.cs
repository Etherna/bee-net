﻿using Etherna.BeeNet;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_5_1.GatewayApi
{
    public class BZZTest : BaseTest_Gateway_v3_0_1
    {
        [Fact]
        public async Task UploadFileAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.UploadFileAsync("batchId");


            // Assert 
        }

        [Fact]
        public async Task GetFileAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);


            // Assert 
        }

        [Fact]
        public async Task GetFilePathAsync()
        {
            // Arrange 
            var reference = await UploadFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.GetFileAsync(reference);


            // Assert 
        }

    }
}
