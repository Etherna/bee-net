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
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Etherna.BeeNet.Chunks
{
    public class ChunkEncrypterTest
    {
        // Internal records.
        public record CanEncryptDecryptTestElement(byte[] SpanData);

        // Data.
        public static IEnumerable<object[]> CanEncryptDecryptTest
        {
            get
            {
                var tests = new List<CanEncryptDecryptTestElement>();

                // Chunk with Span value < SwarmCac.DataSize
                {
                    var span = SwarmCac.LengthToSpan(1234);
                    var data = new byte[1234];
                    new Random(0).NextBytes(data);

                    tests.Add(new CanEncryptDecryptTestElement(span.Concat(data).ToArray()));
                }

                // Chunk with Span value == SwarmCac.DataSize
                {
                    var span = SwarmCac.LengthToSpan(SwarmCac.DataSize);
                    var data = new byte[SwarmCac.DataSize];
                    new Random(0).NextBytes(data);

                    tests.Add(new CanEncryptDecryptTestElement(span.Concat(data).ToArray()));
                }

                // Chunk with Span value > SwarmCac.DataSize
                {
                    var span = SwarmCac.LengthToSpan(1575194); //~1.6MB
                    var data = new byte[448];
                    new Random(0).NextBytes(data);

                    tests.Add(new CanEncryptDecryptTestElement(span.Concat(data).ToArray()));
                }

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        [Theory, MemberData(nameof(CanEncryptDecryptTest))]
        public void CanEncryptDecrypt(CanEncryptDecryptTestElement test)
        {
            var hasher = new Hasher();

            var key = ChunkEncrypter.EncryptChunk(
                test.SpanData[..SwarmCac.SpanSize],
                test.SpanData[SwarmCac.SpanSize..],
                null,
                hasher,
                out var encryptedSpanData);

            ChunkEncrypter.DecryptChunk(
                encryptedSpanData[..SwarmCac.SpanSize],
                encryptedSpanData[SwarmCac.SpanSize..],
                key,
                hasher,
                out var decryptedSpanData);

            // Assert.
            Assert.Equal(test.SpanData, decryptedSpanData);
        }
    }
}