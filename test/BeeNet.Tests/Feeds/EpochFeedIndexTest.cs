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

using System;
using Xunit;

namespace Etherna.BeeNet.Feeds
{
    public class EpochFeedIndexTest
    {
        // Tests.
        [Fact]
        public void VerifyMaxStart()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new EpochFeedIndex(8_589_934_592, 0));
        }

        [Fact]
        public void VerifyMaxLevel()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new EpochFeedIndex(0, 33));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(2_147_483_648, 31, 2_147_483_648)]
        [InlineData(3_456_789_012, 31, 2_147_483_648)]
        [InlineData(0, 32, 0)]
        [InlineData(2_147_483_648, 32, 0)]
        [InlineData(4_294_967_296, 32, 4_294_967_296)]
        [InlineData(8_589_934_591, 32, 4_294_967_296)]
        public void StartNormalization(ulong start, byte level, ulong expected)
        {
            var index = new EpochFeedIndex(start, level);
            Assert.Equal(expected, index.Start);
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(1, 0, false)]
        [InlineData(2, 1, false)]
        [InlineData(4, 1, true)]
        [InlineData(0, 32, true)]
        [InlineData(4_294_967_296, 32, false)]
        public void IsLeft(ulong start, byte level, bool expected)
        {
            var index = new EpochFeedIndex(start, level);
            Assert.Equal(expected, index.IsLeft);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(1, 0, true)]
        [InlineData(2, 1, true)]
        [InlineData(4, 1, false)]
        [InlineData(0, 32, false)]
        [InlineData(4_294_967_296, 32, true)]
        public void IsRight(ulong start, byte level, bool expected)
        {
            var index = new EpochFeedIndex(start, level);
            Assert.Equal(expected, index.IsRight);
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, 0, 0)]
        [InlineData(2, 1, 0, 1)]
        [InlineData(4, 1, 4, 1)]
        [InlineData(0, 32, 0, 32)]
        [InlineData(4_294_967_296, 32, 0, 32)]
        public void Left(ulong start, byte level, ulong expectedStart, byte expectedLevel)
        {
            var index = new EpochFeedIndex(start, level);
            var leftIndex = index.Left;
            Assert.Equal(expectedStart, leftIndex.Start);
            Assert.Equal(expectedLevel, leftIndex.Level);
        }

        [Theory]
        [InlineData(0, 0, 1, 0)]
        [InlineData(1, 0, 1, 0)]
        [InlineData(2, 1, 2, 1)]
        [InlineData(4, 1, 6, 1)]
        [InlineData(0, 32, 4_294_967_296, 32)]
        [InlineData(4_294_967_296, 32, 4_294_967_296, 32)]
        public void Right(ulong start, byte level, ulong expectedStart, byte expectedLevel)
        {
            var index = new EpochFeedIndex(start, level);
            var rightIndex = index.Right;
            Assert.Equal(expectedStart, rightIndex.Start);
            Assert.Equal(expectedLevel, rightIndex.Level);
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(0, 1, 2)]
        [InlineData(0, 32, 4_294_967_296)]
        public void Length(ulong start, byte level, ulong expected)
        {
            var index = new EpochFeedIndex(start, level);
            Assert.Equal(expected, index.Length);
        }

        [Theory]
        [InlineData(0, 0, new byte[] { 173, 49, 94, 32, 157, 214, 37, 22, 171, 140, 125, 28, 45, 140, 60, 32, 101, 37, 80, 30, 190, 249, 29, 18, 195, 68, 49, 249, 234, 37, 83, 113 })]
        [InlineData(0, 1, new byte[] { 251, 40, 138, 229, 98, 70, 144, 153, 126, 77, 233, 207, 177, 166, 218, 44, 127, 113, 59, 174, 156, 119, 11, 133, 184, 56, 90, 25, 174, 90, 175, 133 })]
        [InlineData(0, 32, new byte[] { 42, 40, 146, 107, 120, 198, 38, 173, 183, 162, 73, 162, 62, 151, 105, 191, 3, 139, 82, 68, 126, 96, 84, 48, 134, 167, 151, 249, 179, 6, 28, 112 })]
        [InlineData(4_294_967_296, 0, new byte[] { 10, 81, 169, 21, 123, 21, 96, 75, 132, 136, 101, 165, 120, 209, 156, 5, 176, 74, 30, 5, 191, 84, 113, 247, 122, 4, 144, 222, 33, 151, 100, 113 })]
        [InlineData(4_294_967_296, 1, new byte[] { 3, 48, 42, 58, 75, 159, 28, 60, 34, 143, 230, 13, 57, 78, 229, 146, 36, 135, 120, 28, 76, 130, 175, 49, 184, 192, 11, 103, 45, 126, 227, 135 })]
        [InlineData(4_294_967_296, 32, new byte[] { 138, 221, 60, 55, 69, 133, 200, 248, 94, 216, 56, 133, 121, 93, 5, 7, 253, 249, 194, 232, 213, 22, 134, 6, 183, 249, 62, 225, 177, 8, 9, 103 })]
        public void MarshalBinary(ulong start, byte level, byte[] expected)
        {
            var index = new EpochFeedIndex(start, level);
            Assert.Equal(expected, index.MarshalBinary);
        }

        [Theory]
        [InlineData(1, 0, 0, false)]
        [InlineData(1, 0, 1, true)]
        [InlineData(1, 0, 2, false)]
        [InlineData(0, 32, 0, true)]
        [InlineData(0, 32, 4_294_967_295, true)]
        [InlineData(0, 32, 4_294_967_296, false)]
        [InlineData(4_294_967_296, 32, 4_294_967_295, false)]
        [InlineData(4_294_967_296, 32, 4_294_967_296, true)]
        public void ContainsTime(ulong start, byte level, ulong at, bool expected)
        {
            var index = new EpochFeedIndex(start, level);
            var result = index.ContainsTime(at);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetChildAtVerifyMinLevel()
        {
            var index = new EpochFeedIndex(0, 0);
            Assert.Throws<InvalidOperationException>(
                () => index.GetChildAt(0));
        }

        [Theory]
        [InlineData(2, 1, 1)]
        [InlineData(2, 1, 4)]
        public void GetChildAtVerifyAtBounds(ulong start, byte level, ulong at)
        {
            var index = new EpochFeedIndex(start, level);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => index.GetChildAt(at));
        }

        [Theory]
        [InlineData(2, 1, 2, 2, 0)]
        [InlineData(2, 1, 3, 3, 0)]
        [InlineData(0, 32, 2000000000, 0, 31)]
        [InlineData(0, 32, 3000000000, 2_147_483_648, 31)]
        [InlineData(4_294_967_296, 32, 6000000000, 4_294_967_296, 31)]
        [InlineData(4_294_967_296, 32, 7000000000, 6_442_450_944, 31)]
        public void GetChildAt(ulong start, byte level, ulong at, ulong expectedStart, byte expectedLevel)
        {
            var index = new EpochFeedIndex(start, level);
            var childIndex = index.GetChildAt(at);
            Assert.Equal(expectedStart, childIndex.Start);
            Assert.Equal(expectedLevel, childIndex.Level);
        }

        [Fact]
        public void GetNextVerifyMinAt()
        {
            var index = new EpochFeedIndex(2, 1);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => index.GetNext(1));
        }

        [Theory]
        [InlineData(2, 1, 2, 2, 0)]
        [InlineData(2, 1, 3, 3, 0)]
        [InlineData(2, 1, 4, 4, 2)]
        public void GetNext(ulong start, byte level, ulong at, ulong expectedStart, byte expectedLevel)
        {
            var index = new EpochFeedIndex(start, level);
            var nextIndex = (EpochFeedIndex)index.GetNext(at);
            Assert.Equal(expectedStart, nextIndex.Start);
            Assert.Equal(expectedLevel, nextIndex.Level);
        }

        [Fact]
        public void GetParentVerifyMaxLevel()
        {
            var index = new EpochFeedIndex(0, 32);
            Assert.Throws<InvalidOperationException>(
                () => index.Parent);
        }

        [Theory]
        [InlineData(2, 1, 0, 2)]
        [InlineData(4, 1, 4, 2)]
        public void GetParent(ulong start, byte level, ulong expectedStart, byte expectedLevel)
        {
            var index = new EpochFeedIndex(start, level);
            var parentIndex = index.Parent;
            Assert.Equal(expectedStart, parentIndex.Start);
            Assert.Equal(expectedLevel, parentIndex.Level);
        }

        [Theory]
        [InlineData(0, 8_589_934_591)]
        [InlineData(4_294_967_295, 4_294_967_296)]
        public void LowestCommonAncestorVerifyMaxLevel(ulong t0, ulong t1)
        {
            Assert.Throws<InvalidOperationException>(
                () => EpochFeedIndex.LowestCommonAncestor(t0, t1));
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 1, 0, 1)]
        [InlineData(0, 2, 0, 2)]
        [InlineData(1, 2, 0, 2)]
        [InlineData(1, 3, 0, 2)]
        [InlineData(5, 6, 4, 2)]
        public void LowestCommonAncestor(ulong t0, ulong t1, ulong expectedStart, byte expectedLevel)
        {
            var lcaIndex = EpochFeedIndex.LowestCommonAncestor(t0, t1);
            Assert.Equal(expectedStart, lcaIndex.Start);
            Assert.Equal(expectedLevel, lcaIndex.Level);
        }
    }
}
