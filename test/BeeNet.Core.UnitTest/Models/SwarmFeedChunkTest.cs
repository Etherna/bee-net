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

using Etherna.BeeNet.Hashing;
using Xunit;

namespace Etherna.BeeNet.Models
{
    public class SwarmFeedChunkTest
    {
        // Tests.

        [Fact]
        public void BuildIdentifier()
        {
            SwarmFeedTopic topic = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
            var index = new SwarmEpochFeedIndex(2, 1, new Hasher());

            var result = SwarmFeedChunkBase.BuildIdentifier(topic, index, new Hasher());

            Assert.Equal(
                new byte[] { 229, 116, 252, 141, 32, 73, 147, 48, 181, 92, 124, 96, 74, 217, 20, 163, 90, 16, 124, 66, 174, 221, 76, 184, 135, 58, 193, 210, 235, 104, 138, 215 },
                result);
        }
        
        [Fact]
        public void BuildHash()
        {
            EthAddress account = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
            SwarmSocIdentifier identifier = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        
            var result = SwarmSoc.BuildHash(identifier, account, new Hasher());
        
            Assert.Equal(
                "854f1dd0c708a544e282b25b9f9c1d353dca28e352656993ab3c2c17b384a86f",
                result);
        }
    }
}
