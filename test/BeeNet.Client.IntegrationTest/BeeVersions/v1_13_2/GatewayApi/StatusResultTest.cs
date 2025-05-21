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
    public class StatusResultTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetReserveStateAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.GetReserveStateAsync();


            // Assert
            Assert.True(reserveState.Commitment > 0);
            Assert.True(reserveState.Radius > 0);
        }

        [Fact]
        public async Task GetChainStateAsync()
        {
            // Arrange 


            // Act 
            var chainState = await beeNodeClient.GetChainStateAsync();


            // Assert
            Assert.True(chainState.Block > 0);
        }

        [Fact]
        public async Task GetNodeInfoAsync()
        {
            // Arrange 


            // Act 
            var nodeInfo = await beeNodeClient.GetNodeInfoAsync();


            // Assert
            Assert.True(nodeInfo.ChequebookEnabled);
            Assert.True(nodeInfo.SwapEnabled);
        }

        [IgnoreOtherVersionFact(testVersion: version)]
        public async Task GetHealthAsync()
        {
            // Act 
            var healthAsync = await beeNodeClient.GetHealthAsync();

            // Assert
            Assert.Equal("5.0.0", healthAsync.ApiVersion);
            Assert.True(healthAsync.IsStatusOk);
            Assert.StartsWith("1.13.2-", healthAsync.Version);
        }
    }
}