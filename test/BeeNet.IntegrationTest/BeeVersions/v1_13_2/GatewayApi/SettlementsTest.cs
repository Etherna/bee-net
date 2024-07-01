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

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class SettlementsTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetAllSettlementsAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;

            // Act 
            var reserveState = await beeNodeClient.GetAllSettlementsAsync();


            // Assert
            Assert.Contains(reserveState.Settlements, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetAllTimeSettlementsAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var reserveState = await beeNodeClient.GetAllTimeSettlementsAsync();


            // Assert
            Assert.Contains(reserveState.Settlements, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetSettlementsWithPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var reserveState = await beeNodeClient.GetSettlementsWithPeerAsync(peerId);


            // Assert
            Assert.Equal(peerId, reserveState.Peer);
        }
    }
}
