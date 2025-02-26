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

using System.Threading.Tasks;
using Xunit;

namespace BeeNet.Client.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class ConnectivityTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GetAddressesAsync();


            // Assert 
        }

        [Fact]
        public async Task GetBlocklistedPeerAddressesAsync()
        {
            // Act.
            await beeNodeClient.GetBlocklistedPeerAddressesAsync();
        }

        /*
        [Fact]
        public async Task ConnectToPeerAsync()
        {
            // Arrange 
            var addresses = await beeNodeClient.GetAddressesAsync();


            // Act 
            var result = await beeNodeClient.ConnectToPeerAsync(""); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        */

        [Fact]
        public async Task GetAllPeerAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GetAllPeerAddressesAsync();


            // Assert 
            Assert.NotEmpty(result);
        }

        /*
        [Fact]
        public async Task DeletePeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.DeletePeerAsync("address"); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        

        [Fact]
        public async Task TryConnectToPeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.TryConnectToPeerAsync("address"); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        */

        [Fact]
        public async Task GetSwarmTopologyAsync()
        {
            // Act.
            await beeNodeClient.GetSwarmTopologyAsync();
        }

        [Fact]
        public async Task GetSet_WelcomeMessageAsync()
        {
            // Arrange 
            var message = "MyTEst message welcome";


            // Act 
            await beeNodeClient.SetWelcomeMessageAsync(message);


            // Assert 
            var result = await beeNodeClient.GetWelcomeMessageAsync();
            Assert.Equal(message, result);
        }
    }
}
