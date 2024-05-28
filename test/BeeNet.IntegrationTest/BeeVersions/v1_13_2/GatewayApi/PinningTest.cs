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
    public class PinningTest : BaseTest_Gateway_v5_0_0
    {
        
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Act 
            var result = await beeNodeClient.CreatePinAsync(reference);


            // Assert 
            Assert.True(result.Code == 200 || result.Code == 201);
        }

        [Fact]
        public async Task DeletePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.CreatePinAsync(reference);


            // Act 
            var result = await beeNodeClient.DeletePinAsync(reference);


            // Assert 
            Assert.Equal(200, result.Code);
        }

        [Fact]
        public async Task GetPinStatusAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.CreatePinAsync(reference);
            await Task.Delay(60000);


            // Act 
            await beeNodeClient.GetPinStatusAsync(reference);
        }

        [Fact]
        public async Task GetAllPinsAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.CreatePinAsync(reference);
            await Task.Delay(60000);


            // Act 
            var results = await beeNodeClient.GetAllPinsAsync();


            // Assert 
            Assert.NotEmpty(results);
        }
        
    }
}
