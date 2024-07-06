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
            string? expectedRelativePath)
        {
            public string InputString { get; } = inputString;
            public SwarmHash ExpectedHash { get; } = expectedHash;
            public string? ExpectedRelativePath { get; } = expectedRelativePath;
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
                    new SwarmAddress(SwarmHash.Zero, "Im/a/relative/path"),
                    "0000000000000000000000000000000000000000000000000000000000000000/Im/a/relative/path"));
                
                // With path with root.
                tests.Add(new(
                    new SwarmAddress(SwarmHash.Zero, "/I/have/a/root"),
                    "0000000000000000000000000000000000000000000000000000000000000000/I/have/a/root"));
                
                // With special chars.
                tests.Add(new(
                    new SwarmAddress(SwarmHash.Zero, "I have a % of special\\chars!"),
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
                    "Im/a/path"));
                
                // With initial root and path.
                tests.Add(new(
                    "/0000000000000000000000000000000000000000000000000000000000000000/Im/a/path",
                    SwarmHash.Zero,
                    "Im/a/path"));
                
                // With special chars.
                tests.Add(new(
                    "0000000000000000000000000000000000000000000000000000000000000000/I have a % of special\\chars!",
                    SwarmHash.Zero,
                    "I have a % of special\\chars!"));

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
            Assert.Equal(test.ExpectedRelativePath, result.Path);
        }
    }
}