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

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Etherna.BeeNet.Models
{
    public class SwarmAddressTest
    {
        // Internal classes.
        public class AddressToStringTestElement(
            SwarmAddress address,
            string expectedString)
        {
            public SwarmAddress Address { get; } = address;
            public string ExpectedString { get; } = expectedString;
        }

        public class StringToAddressTestElement(
            string inputString,
            SwarmHash expectedHash,
            Uri? expectedRelativePath)
        {
            public string InputString { get; } = inputString;
            public SwarmHash ExpectedHash { get; } = expectedHash;
            public Uri? ExpectedRelativePath { get; } = expectedRelativePath;
        }

        // Data.
        public static IEnumerable<object[]> AddressToStringTests
        {
            get
            {
                var tests = new List<AddressToStringTestElement>();
                
                // Only hash.
                tests.Add(new(
                    new SwarmAddress(SwarmHash.Zero),
                    "0000000000000000000000000000000000000000000000000000000000000000/"));
                
                // With path without root.
                tests.Add(new(
                    new SwarmAddress(SwarmHash.Zero, new Uri("Im/a/relative/path", UriKind.Relative)),
                    "0000000000000000000000000000000000000000000000000000000000000000/Im/a/relative/path"));
                
                // With path with root.
                tests.Add(new(
                    new SwarmAddress(SwarmHash.Zero, new Uri("/I/have/a/root", UriKind.Relative)),
                    "0000000000000000000000000000000000000000000000000000000000000000/I/have/a/root"));
                
                // With special chars.
                tests.Add(new(
                    new SwarmAddress(SwarmHash.Zero, new Uri("I have a % of special\\chars!", UriKind.Relative)),
                    "0000000000000000000000000000000000000000000000000000000000000000/I have a % of special\\chars!"));

                return tests.Select(t => new object[] { t });
            }
        }
        
        public static IEnumerable<object[]> StringToAddressTests
        {
            get
            {
                var tests = new List<StringToAddressTestElement>();
                
                // Only hash without ending slash.
                tests.Add(new(
                    "0000000000000000000000000000000000000000000000000000000000000000",
                    SwarmHash.Zero,
                    null));
                
                // Only hash with ending slash.
                tests.Add(new(
                    "0000000000000000000000000000000000000000000000000000000000000000/",
                    SwarmHash.Zero,
                    null));
                
                // With initial root.
                tests.Add(new(
                    "/0000000000000000000000000000000000000000000000000000000000000000",
                    SwarmHash.Zero,
                    null));
                
                // With initial root and ending slash.
                tests.Add(new(
                    "/0000000000000000000000000000000000000000000000000000000000000000/",
                    SwarmHash.Zero,
                    null));
                
                // With path.
                tests.Add(new(
                    "0000000000000000000000000000000000000000000000000000000000000000/Im/a/path",
                    SwarmHash.Zero,
                    new Uri("Im/a/path", UriKind.Relative)));
                
                // With initial root and path.
                tests.Add(new(
                    "/0000000000000000000000000000000000000000000000000000000000000000/Im/a/path",
                    SwarmHash.Zero,
                    new Uri("Im/a/path", UriKind.Relative)));
                
                // With special chars.
                tests.Add(new(
                    "0000000000000000000000000000000000000000000000000000000000000000/I have a % of special\\chars!",
                    SwarmHash.Zero,
                    new Uri("I have a % of special\\chars!", UriKind.Relative)));

                return tests.Select(t => new object[] { t });
            }
        }
        
        // Tests.
        [Theory, MemberData(nameof(AddressToStringTests))]
        public void AddressToString(AddressToStringTestElement test)
        {
            var result = test.Address.ToString();
            
            Assert.Equal(test.ExpectedString, result);
        }

        [Theory, MemberData(nameof(StringToAddressTests))]
        public void StringToAddress(StringToAddressTestElement test)
        {
            var result = new SwarmAddress(test.InputString);
            
            Assert.Equal(test.ExpectedHash, result.Hash);
            Assert.Equal(test.ExpectedRelativePath, result.RelativePath);
        }

        [Fact]
        public void ExceptWithAbsoluteUri()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var absolutePath = new Uri("https://etherna.io/", UriKind.Absolute);
                return new SwarmAddress(SwarmHash.Zero, absolutePath);
            });
        }
    }
}