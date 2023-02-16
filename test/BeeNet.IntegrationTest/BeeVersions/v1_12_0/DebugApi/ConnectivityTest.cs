using Etherna.BeeNet.DtoModels;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_12_0.DebugApi
{
    public class ConnectivityTest : BaseTest_Debug_v4_0_0
    {
        [Fact]
        public async Task GetAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.GetAddressesAsync();


            // Assert 
            Assert.StartsWith("0x", result.Ethereum);
        }

        [Fact]
        public async Task GetBlocklistedPeerAddressesAsync()
        {
            // Act.
            await beeNodeClient.DebugClient.GetBlocklistedPeerAddressesAsync();
        }

        /*
        [Fact]
        public async Task ConnectToPeerAsync()
        {
            // Arrange 
            var addresses = await beeNodeClient.DebugClient.GetAddressesAsync();


            // Act 
            var result = await beeNodeClient.DebugClient.ConnectToPeerAsync(""); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        */

        [Fact]
        public async Task GetAllPeerAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();


            // Assert 
            Assert.NotEmpty(result);
        }

        /*
        [Fact]
        public async Task DeletePeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.DebugClient.DeletePeerAsync("address"); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        

        [Fact]
        public async Task TryConnectToPeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.DebugClient.TryConnectToPeerAsync("address"); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        */

        [Fact]
        public async Task GetSwarmTopologyAsync()
        {
            // Act.
            await beeNodeClient.DebugClient.GetSwarmTopologyAsync();
        }

        [Fact]
        public async Task GetSet_WelcomeMessageAsync()
        {
            // Arrange 
            var message = "MyTEst message welcome";


            // Act 
            await beeNodeClient.DebugClient.SetWelcomeMessageAsync(message);


            // Assert 
            var result = await beeNodeClient.DebugClient.GetWelcomeMessageAsync();
            Assert.Equal(message, result);
        }
    }
}
