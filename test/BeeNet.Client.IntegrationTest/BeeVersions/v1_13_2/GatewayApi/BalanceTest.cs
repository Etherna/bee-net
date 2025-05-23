﻿// Copyright 2021-present Etherna SA
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

namespace BeeNet.Client.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class BalanceTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetAllBalancesAsync()
        {
            // Act.
            await beeNodeClient.GetAllBalancesAsync();
        }

        [Fact]
        public async Task GetAllConsumedBalancesAsync()
        {
            // Act.
            await beeNodeClient.GetAllConsumedBalancesAsync();
        }

        [Fact]
        public async Task GetBalanceWithPeerAsync()
        {
            // Arrange .
            var peers = await beeNodeClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();

            // Act.
            var balance = await beeNodeClient.GetBalanceWithPeerAsync(peerId);
        }


        [Fact]
        public async Task GetConsumedBalanceWithPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var balance = await beeNodeClient.GetConsumedBalanceWithPeerAsync(peerId);
        }
    }
}
