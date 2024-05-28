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

using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class FeedTest : BaseTest_Gateway_v5_0_0
    {

        [Fact]
        public async Task CreateFeedAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var addresses = await beeNodeClient.GatewayClient.GetAddressesAsync();
            var topic = "cf880b8eeac5093fa27b0825906c600685";


            // Act 
            var result = await beeNodeClient.GatewayClient.CreateFeedAsync(addresses.Ethereum.Replace("0x", ""), topic, batch);


            // Assert 
            Assert.NotEmpty(result);
        }

        /*
        [Fact]
        public async Task GetFeedAsync()
        {
            // Arrange 
            //var batch = await beeNodeClient.GatewayClient.BuyPostageBatchAsync(500, 32);
            //await Task.Delay(180000);
            var batch = "855f9389cf57a01369cff81901e4f5191ec03191f9a2d4d806486e9d856f9cdc";
            var addresses = await beeNodeClient.GatewayClient.GetAddressesAsync();
            var topic = "cf880b8eeac5093fa27b0825906c600685";
            var feed = await beeNodeClient.GatewayClient.CreateFeedAsync(addresses.Ethereum.Replace("0x", ""), topic, batch);
            await Task.Delay(180000);
            

            // Act 
            var result = await beeNodeClient.GatewayClient.GetFeedAsync(addresses.Ethereum.Replace("0x", ""), topic);


            // Assert 
            Assert.Equal(feed, result);
        }
        */

    }
}
