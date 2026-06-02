// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
//
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using Xunit;

namespace Etherna.SwarmSdk.Models
{
    public class SwarmSocSignatureTest
    {
        private static byte[] SampleBytes =>
            Enumerable.Range(0, SwarmSocSignature.SignatureSize).Select(i => (byte)i).ToArray();

        // Tests.
        [Fact]
        public void RoundTripsThroughByteArray()
        {
            var bytes = SampleBytes;

            var signature = new SwarmSocSignature(bytes);

            Assert.Equal(bytes, signature.ToByteArray());
        }

        [Fact]
        public void RoundTripsThroughString()
        {
            var signature = new SwarmSocSignature(SampleBytes);

            var asString = signature.ToString();
            var reparsed = new SwarmSocSignature(asString);

            Assert.Equal(signature, reparsed);
            Assert.Equal(signature.ToByteArray(), reparsed.ToByteArray());
        }

        [Theory]
        [InlineData(SwarmSocSignature.SignatureSize - 1)]
        [InlineData(SwarmSocSignature.SignatureSize + 1)]
        public void ConstructorRejectsInvalidLength(int length)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SwarmSocSignature(new byte[length]));
        }

        [Fact]
        public void ConstructorRejectsInvalidHex()
        {
            Assert.Throws<ArgumentException>(() => new SwarmSocSignature("not-hex"));
        }

        [Fact]
        public void IsValidSignatureChecksLength()
        {
            Assert.True(SwarmSocSignature.IsValidSignature(SampleBytes));
            Assert.False(SwarmSocSignature.IsValidSignature(new byte[10]));
            Assert.False(SwarmSocSignature.IsValidSignature("zzzz"));
        }

        [Fact]
        public void EqualityIsByValue()
        {
            var a = new SwarmSocSignature(SampleBytes);
            var b = new SwarmSocSignature(SampleBytes);
            var different = SampleBytes;
            different[0] ^= 0xFF;
            var c = new SwarmSocSignature(different);

            Assert.True(a == b);
            Assert.True(a.Equals(b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.True(a != c);
        }

        [Fact]
        public void TryParseHandlesValidAndInvalid()
        {
            var valid = new SwarmSocSignature(SampleBytes).ToString();

            Assert.True(SwarmSocSignature.TryParse(valid, null, out var parsed));
            Assert.Equal(SampleBytes, parsed.ToByteArray());

            Assert.False(SwarmSocSignature.TryParse(null, null, out _));
            Assert.False(SwarmSocSignature.TryParse("   ", null, out _));
            Assert.False(SwarmSocSignature.TryParse("nope", null, out _));
        }

        [Fact]
        public void ConversionOperatorsRoundTrip()
        {
            SwarmSocSignature fromBytes = SampleBytes;
            SwarmSocSignature fromString = new SwarmSocSignature(SampleBytes).ToString();

            Assert.Equal(SampleBytes, (byte[])fromBytes);
            Assert.Equal(fromBytes, fromString);
        }
    }
}
