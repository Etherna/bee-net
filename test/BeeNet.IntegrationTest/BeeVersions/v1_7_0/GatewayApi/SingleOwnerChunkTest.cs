﻿using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class SingleOwnerChunkTest : BaseTest_Gateway_v3_0_2
    {
        [Fact]
        public async Task SendPssAsync()
        {
            // Arrange 
            var addresses = await beeNodeClient.DebugClient.GetAddressesAsync();

            // Act 
            await beeNodeClient.GatewayClient.UploadSocAsync(addresses.Ethereum.Replace("0x", ""), "cf880b8eeac5093fa27b0825906c600685", "cf880b8eeac5093fa27b0825906c600685");


            // Assert 
        }
    }
}