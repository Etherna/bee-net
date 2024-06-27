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

using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class StewardshipTest : BaseTest_Gateway_v5_0_0
    {
        
        [Fact]
        public async Task CheckIsContentAvailableAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();
            await Task.Delay(180000);

            // Act 
            var result = await beeNodeClient.CheckIsContentAvailableAsync(reference);


            // Assert 
            Assert.True(result.IsRetrievable);
        }

        [Fact]
        public async Task ReuploadContentAsync()
        {
            // Arrange 
            var reference = await UploadBZZFileAndGetReferenceAsync();


            // Act 
            await beeNodeClient.ReuploadContentAsync(reference);
        }

    }
}
