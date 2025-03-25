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

using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.Client.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class FeedTest : BaseTest_Gateway_v5_0_0
    {

        [Fact]
        public async Task CreateFeedAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var addresses = await beeNodeClient.GetAddressesAsync();
            var topic = "cf880b8eeac5093fa27b0825906c600685";

            // Act 
            var result = await beeNodeClient.CreateFeedAsync(addresses.Ethereum, topic.HexToByteArray(), batch);
        }

        /*
        [Fact]
        public async Task GetFeedAsync()
        {
            // Arrange 
            //var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            //await Task.Delay(180000);
            var batch = "855f9389cf57a01369cff81901e4f5191ec03191f9a2d4d806486e9d856f9cdc";
            var addresses = await beeNodeClient.GetAddressesAsync();
            var topic = "cf880b8eeac5093fa27b0825906c600685";
            var feed = await beeNodeClient.CreateFeedAsync(addresses.Ethereum.Replace("0x", ""), topic, batch);
            await Task.Delay(180000);
            

            // Act 
            var result = await beeNodeClient.GetFeedAsync(addresses.Ethereum.Replace("0x", ""), topic);


            // Assert 
            Assert.Equal(feed, result);
        }
        */

    }
}
