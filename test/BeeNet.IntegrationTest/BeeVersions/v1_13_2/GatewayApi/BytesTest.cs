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

using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class BytesTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task UploadBytesAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);


            // Act 
            var reference = await beeNodeClient.UploadBytesAsync(batchId: batch, body: File.OpenRead(pathTestFileForUpload));


            // Assert
            var result = await beeNodeClient.GetBytesAsync(reference);
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }

        [Fact]
        public async Task GetBytesAsync()
        {
            // Arrange 
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var reference = await beeNodeClient.UploadBytesAsync(batchId: batch, body: File.OpenRead(pathTestFileForUpload));


            // Act 
            var result = await beeNodeClient.GetBytesAsync(reference);


            // Assert
            StreamReader reader = new(result);
            Assert.Equal(File.ReadAllText(pathTestFileForUpload), reader.ReadToEnd());
        }
        
    }
}
