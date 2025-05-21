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

using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Hashing
{
    public class PipelineTest
    {
        [Theory]
        [InlineData(0, "b34ca8c22b9e982354f9c7f50b470d66db428d880c8a904d5fe4ec9713171526")]         // 0B
        [InlineData(1, "73d7beee42b30991aff05c6f693e6708120aaa513e78e7c0798b5ace1687e4d3")]         // 1B
        [InlineData(31, "3ccd1c9dab870180849f8709b3c0da081d0ddf03606b1091086141e10eb97d63")]        // hashSize -1
        [InlineData(32, "8565c6c2f3f185ad7a1db712f24c267d8abdfbcb8f495a399a00a806d5885f02")]        // hashSize
        [InlineData(33, "3617d23a0b51a5cc6efac60fddb3b34aea1cb9b6775dd9e05b08ca04260b0d70")]        // hashSize +1
        [InlineData(4095, "499adde5b6ddb224a95c4917691a032cc2a3af0b40bef9b03bb23d684c7336b0")]      // maxChunkData -1
        [InlineData(4096, "ddf10d58bc29ff8aa4596d0d6f1c7ad4dc96b422c1f8879f22fbd5cb62c63fac")]      // maxChunkData
        [InlineData(4097, "43163aa5554dc0ba611a5f573c249be9258f50ede812a860f1b6c5245de0781e")]      // maxChunkData +1
        [InlineData(8191, "0716bfe0b1575d710fa1abe814a391cae8f745ab047a6e67ec311e6d3c4062e2")]      // 2*maxChunkData -1
        [InlineData(8192, "72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e")]      // 2*maxChunkData
        [InlineData(8193, "c3641af1c230df638aaf0ec27f3ee8481b5be39d122d68a6fc4acae2d2c0de32")]      // 2*maxChunkData +1
        [InlineData(524287, "1e394e0c4c1883024b3ba72cbc58b603ae3fc0e61108bfdaeefc35e8a15cb228")]    // 128*maxChunkData -1
        [InlineData(524288, "2729f8f847f610297aad7206e627d48550d10c729c8cbb6754e276f5e8598255")]    // 128*maxChunkData
        [InlineData(524289, "7a40111280e319e96b98816bca8258f4f55fd304cb6b7dbc0fb837d5bc223e34")]    // 128*maxChunkData +1
        [InlineData(1048575, "970cff0963f24d525045227779a8592afef35ad6cca4be31fdb4b0ca4ed76cb5")]   // 1MB -1
        [InlineData(1048576, "e529cf3f25310dcda26591ec39e91e355f741ba590c43af6176f596fce245a0e")]   // 1MB
        [InlineData(1048577, "abcb683950626cfffffa1520c10895e5829e26144fa2448f831065d4a612d481")]   // 1MB +1
        public async Task ProduceCorrectPlainHash(int inputDataSize, SwarmHash expectedHash)
        {
            var data = new byte[inputDataSize];
            new Random(0).NextBytes(data);
            var pipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                new FakeChunkStore(),
                new FakePostageStamper(),
                RedundancyLevel.None,
                false,
                0,
                null);

            var result = await pipeline.HashDataAsync(data);
            
            Assert.Equal(expectedHash, result.Hash);
        }
    }
}