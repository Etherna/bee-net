// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Models;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Services
{
    public class CalculatorServiceTest
    {
        [Fact]
        public async Task GetFileHashTest()
        {
            var filePath = "/home/mirkodc/Desktop/test.txt";
            
            await using var fileStream = File.OpenRead(filePath);
            var fileService = new CalculatorService();
            var result = await fileService.EvaluateFileUploadAsync(
                fileStream,
                "text/plain",
                Path.GetFileName(filePath),
                false,
                RedundancyLevel.None);
            
            Assert.Equal("7bf7cfd6f406a27314837eafcc0ae79b0b296dc4f8aa163c822691f9c3234616", result.Address);
            Assert.Equal(17, result.RequiredPostageBatchDepth);
        }
    }
}