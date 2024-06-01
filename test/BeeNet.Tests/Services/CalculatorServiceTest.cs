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
            await using var fileStream = File.OpenRead("/home/mirkodc/Downloads/Etherna presentation for Swarm 2.0.mp4");
            var fileService = new CalculatorService();
            var result = await fileService.EvaluateFileUploadAsync(
                fileStream,
                "",
                null,
                false,
                RedundancyLevel.None);
            
            Assert.Equal("615c9964d2cdf13c0e557cf8821fe93ae15cf4ed40e3312a5b312d0a4ead9e87", result.Address);
            Assert.Equal(19, result.RequiredPostageBatchDepth);
        }
    }
}