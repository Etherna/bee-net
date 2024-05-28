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

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class BalanceTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetAllBalancesAsync()
        {
            // Act.
            await beeNodeClient.GatewayClient.GetAllBalancesAsync();
        }

        [Fact]
        public async Task GetAllConsumedBalancesAsync()
        {
            // Act.
            await beeNodeClient.GatewayClient.GetAllConsumedBalancesAsync();
        }

        [Fact]
        public async Task GetBalanceWithPeerAsync()
        {
            // Arrange .
            var peers = await beeNodeClient.GatewayClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();

            // Act.
            var balance = await beeNodeClient.GatewayClient.GetBalanceWithPeerAsync(peerId);

            // Assert.
            Assert.Equal(peerId, balance.Peer);
        }


        [Fact]
        public async Task GetConsumedBalanceWithPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.GatewayClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var balance = await beeNodeClient.GatewayClient.GetConsumedBalanceWithPeerAsync(peerId);


            // Assert
            Assert.Equal(peerId, balance.Peer);
        }
    }
}
