using System;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class ConnectivityTest : BaseTest_Gateway_v3_0_2
    {
        [Fact]
        public async Task GetAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetAddressesAsync();


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetBlocklistedPeerAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetBlocklistedPeerAddressesAsync();


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ConnectToPeerAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.ConnectToPeerAsync("address");


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetAllPeerAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetAllPeerAddressesAsync();


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task DeletePeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.DeletePeerAsync("address");


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task TryConnectToPeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.TryConnectToPeerAsync("address");


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetSwarmTopologyAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.GetSwarmTopologyAsync();


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task GetWelcomeMessageAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.GetWelcomeMessageAsync();


            // Assert 
            throw new NotImplementedException();
        }

        [Fact]
        public async Task SetWelcomeMessageAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.SetWelcomeMessageAsync("");


            // Assert 
            throw new NotImplementedException();
        }
    }
}
