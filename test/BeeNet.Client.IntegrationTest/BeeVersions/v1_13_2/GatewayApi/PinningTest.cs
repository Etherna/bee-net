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
    public class PinningTest : BaseTest_Gateway_v5_0_0
    {
        
        [Fact]
        public async Task CreatePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            
            // Act 
            await beeNodeClient.CreatePinAsync(reference);
        }

        [Fact]
        public async Task DeletePinAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await beeNodeClient.CreatePinAsync(reference);
            
            // Act 
            await beeNodeClient.DeletePinAsync(reference);
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
