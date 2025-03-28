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

using Xunit;

namespace Etherna.BeeNet.Models
{
    public class SwarmSequenceFeedIndexTest
    {
        // Tests.

        [Theory]
        [InlineData(0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 })]
        [InlineData(1, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 })]
        [InlineData(2, new byte[] { 0, 0, 0, 0, 0, 0, 0, 2 })]
        [InlineData(1000, new byte[] { 0, 0, 0, 0, 0, 0, 3, 232 })]
        [InlineData(18_446_744_073_709_551_615, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 })]
        public void MarshalBinary(ulong value, byte[] expected)
        {
            var index = new SwarmSequenceFeedIndex(value);
            var marshalBinary = index.MarshalBinary();
            Assert.Equal(expected, marshalBinary);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(1000, 1001)]
        public void GetNext(ulong value, ulong expectedValue)
        {
            var index = new SwarmSequenceFeedIndex(value);
            var nextIndex = (SwarmSequenceFeedIndex)index.GetNext(0);
            Assert.Equal(expectedValue, nextIndex.Value);
        }
    }
}