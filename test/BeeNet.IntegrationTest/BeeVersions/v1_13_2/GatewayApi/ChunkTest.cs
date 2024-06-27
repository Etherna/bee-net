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

using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class ChunkTest : BaseTest_Gateway_v5_0_0
    {
        //[Fact]
        //public async Task UploadChunkAsync()
        //{
        //    // Arrange 
        //    var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
        //    var tag = await beeNodeClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
        //    var fs = File.OpenRead("Data/TestFileForUpload_Debug.txt");
        //    await Task.Delay(180000);


        //    // Act 
        //    var result = await beeNodeClient.UploadChunkAsync(batch, tag.Uid, body: fs);


        //    // Assert
        //    //TODO check stream data
        //}

        //[Fact]
        //public async Task GetChunkAsync()
        //{
        //    // Arrange 
        //    var reference = await UploadChunkFileAndGetReferenceAsync();


        //    // Act 
        //    var result = await beeNodeClient.GetChunkAsync(reference);


        //    // Assert
        //}

        //[Fact]
        //public async Task ChunksHeadAsync()
        //{
        //    // Arrange 
        //    var reference = await UploadChunkFileAndGetReferenceAsync();


        //    // Act 
        //    var result = await beeNodeClient.ChunksHeadAsync(reference);


        //    // Assert
        //}

        //[Fact]
        //public async Task DeleteChunkAsync()
        //{
        //    // Arrange 
        //    var reference = await UploadChunkFileAndGetReferenceAsync();


        //    // Act 
        //    var result = await beeNodeClient.DeleteChunkAsync(reference);


        //    // Assert
        //}
        
        /*
        [Fact]
        public async Task GetChunkAsync()
        {
            // Arrange
            var reference = await UploadFileAndGetReferenceAsync();


            // Act
            var result = await beeNodeClient.GetChunkAsync(reference); //TODO address


            // Assert
        }

        [Fact]
        public async Task DeleteChunkAsync()
        {
            // Arrange
            var reference = await UploadFileAndGetReferenceAsync();


            // Act
            var result = await beeNodeClient.DeleteChunkAsync(reference);


            // Assert
        }

        private async Task<string> UploadFileAndGetReferenceAsync()
        {
            var batch = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            var tag = await beeNodeClient.CreateTagAsync("6790b12369e6416a16bf4d5b950e0c61c1b001f1f6e9cfb27cc9ca6e341365b7");
            var fs = File.OpenRead("Data/TestFileForUpload_Debug.txt");
            await Task.Delay(180000);


            // Act
            var result = await beeNodeClient.UploadChunkAsync(batch, tag.Uid, body: fs);

            return result.Reference;
        }

        */
    }
}