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

using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.DebugApi
{
    public class ConnectivityTest : BaseTest_Debug_v5_0_0
    {
        [Fact]
        public async Task GetAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetAddressesAsync();


            // Assert 
            Assert.StartsWith("0x", result.Ethereum);
        }

        [Fact]
        public async Task GetBlocklistedPeerAddressesAsync()
        {
            // Act.
            await beeNodeClient.GatewayClient.GetBlocklistedPeerAddressesAsync();
        }

        /*
        [Fact]
        public async Task ConnectToPeerAsync()
        {
            // Arrange 
            var addresses = await beeNodeClient.GatewayClient.GetAddressesAsync();


            // Act 
            var result = await beeNodeClient.GatewayClient.ConnectToPeerAsync(""); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        */

        [Fact]
        public async Task GetAllPeerAddressesAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.GatewayClient.GetAllPeerAddressesAsync();


            // Assert 
            Assert.NotEmpty(result);
        }

        /*
        [Fact]
        public async Task DeletePeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.DeletePeerAsync("address"); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        

        [Fact]
        public async Task TryConnectToPeerAsync()
        {
            // Arrange 


            // Act 
            await beeNodeClient.GatewayClient.TryConnectToPeerAsync("address"); //TODO where i can take multiAddr correct???


            // Assert 
            throw new NotImplementedException();
        }
        */

        [Fact]
        public async Task GetSwarmTopologyAsync()
        {
            // Act.
            await beeNodeClient.GatewayClient.GetSwarmTopologyAsync();
        }

        [Fact]
        public async Task GetSet_WelcomeMessageAsync()
        {
            // Arrange 
            var message = "MyTEst message welcome";


            // Act 
            await beeNodeClient.GatewayClient.SetWelcomeMessageAsync(message);


            // Assert 
            var result = await beeNodeClient.GatewayClient.GetWelcomeMessageAsync();
            Assert.Equal(message, result);
        }
    }
}
