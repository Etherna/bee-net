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
using Etherna.BeeNet.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BeeNet.Client.IntegrationTest.BeeVersions.v1_13_2
{
    public abstract class BaseTest_Gateway_v5_0_0
    {
        protected BeeClient beeNodeClient = new(
            new Uri(Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://127.0.0.1:1633/"));
        protected string pathTestFileForUpload = "Data/TestFileForUpload_Gateway.txt";
        protected const string version = "4.0.0";

        protected async Task<SwarmHash> UploadBZZFileAndGetReferenceAsync(string filePath = null)
        {
            var (batch, _) = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);

            // Act 
            var result = await beeNodeClient.UploadFileAsync(
                content: File.OpenRead(filePath ?? pathTestFileForUpload),
                batch,
                name: Path.GetFileName(filePath) ?? Path.GetFileName(pathTestFileForUpload),
                contentType: "text/plain",
                isFileCollection: false);

            return result;
        }

        protected async Task<SwarmHash> UploadChunkFileAndGetReferenceAsync()
        {
            var (batch, _) = await beeNodeClient.BuyPostageBatchAsync(500, 32);
            await Task.Delay(180000);
            var fs = File.OpenRead(pathTestFileForUpload);

            // Act 
            var reference = await beeNodeClient.UploadChunkAsync(fs, batch);

            return reference;
        }
    }
}
