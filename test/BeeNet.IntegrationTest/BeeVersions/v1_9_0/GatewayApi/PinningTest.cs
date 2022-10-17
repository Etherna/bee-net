﻿using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_9_0.GatewayApi
{
    public class PinningTest : BaseTest_Gateway_V3_2_0
    {
        
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.CreatePinAsync(reference);


            // Assert 
            Assert.True(result.Code == 200 || result.Code == 201);
        }

        [Fact]
        public async Task DeletePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.GatewayClient.CreatePinAsync(reference);


            // Act 
            var result = await beeNodeClient.GatewayClient.DeletePinAsync(reference);


            // Assert 
            Assert.Equal(200, result.Code);
        }

        [Fact]
        public async Task GetPinStatusAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.GatewayClient.CreatePinAsync(reference);
            await Task.Delay(60000);


            // Act 
            await beeNodeClient.GatewayClient.GetPinStatusAsync(reference);
        }

        [Fact]
        public async Task GetAllPinsAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.GatewayClient.CreatePinAsync(reference);
            await Task.Delay(60000);


            // Act 
            var results = await beeNodeClient.GatewayClient.GetAllPinsAsync();


            // Assert 
            Assert.NotEmpty(results);
        }
        
    }
}