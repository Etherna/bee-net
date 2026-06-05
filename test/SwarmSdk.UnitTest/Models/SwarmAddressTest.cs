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

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Etherna.SwarmSdk.Models
{
    public class SwarmAddressTest
    {
        // Internal classes.
        public record AddressToStringTestElement(
            SwarmAddress Address,
            string ExpectedString);

        public record CombineUrisTestElement(
            SwarmAddress Address,
            SwarmUri[] InputUris,
            SwarmAddress ExpectedAddress);

        public record StringToAddressTestElement(
            string InputString,
            SwarmReference ExpectedReference,
            string ExpectedRelativePath);

        // Data.
        public static IEnumerable<object[]> AddressToStringTests
        {
            get
            {
                var tests = new List<AddressToStringTestElement>
                {
                    // Only hash.
                    new(new SwarmAddress(SwarmReference.PlainZero),
                        "0000000000000000000000000000000000000000000000000000000000000000/"),
                    
                    // With path without root.
                    new(new SwarmAddress(SwarmReference.PlainZero, "Im/a/relative/path"),
                        "0000000000000000000000000000000000000000000000000000000000000000/Im/a/relative/path"),
                    
                    // With path with root.
                    new(new SwarmAddress(SwarmReference.PlainZero, "/I/have/a/root"),
                        "0000000000000000000000000000000000000000000000000000000000000000/I/have/a/root"),
                    
                    // With path with root.
                    new(new SwarmAddress(SwarmReference.PlainZero, "I/have/final/slash/"),
                        "0000000000000000000000000000000000000000000000000000000000000000/I/have/final/slash/"),
                    
                    // With special chars.
                    new(new SwarmAddress(SwarmReference.PlainZero, "I have a % of special\\chars!"),
                        "0000000000000000000000000000000000000000000000000000000000000000/I have a % of special\\chars!")
                };

                return tests.Select(t => new object[] { t });
            }
        }

        public static IEnumerable<object[]> CombineUrisTests
        {
            get
            {
                var ones = new SwarmReference("1111111111111111111111111111111111111111111111111111111111111111");
                var tests = new List<CombineUrisTestElement>
                {
                    // No uris, returns the address itself.
                    new(new SwarmAddress(SwarmReference.PlainZero, "Im/prefix"),
                        [],
                        new SwarmAddress(SwarmReference.PlainZero, "Im/prefix")),

                    // Relative not rooted uri, appended to the prefix.
                    new(new SwarmAddress(SwarmReference.PlainZero, "Im/prefix"),
                        ["sub/path"],
                        new SwarmAddress(SwarmReference.PlainZero, "Im/prefix/sub/path")),

                    // Relative rooted uri, replaces path but keeps reference.
                    new(new SwarmAddress(SwarmReference.PlainZero, "Im/prefix"),
                        ["/rooted/path"],
                        new SwarmAddress(SwarmReference.PlainZero, "/rooted/path")),

                    // Absolute uri, ignores everything composed before.
                    new(new SwarmAddress(SwarmReference.PlainZero, "Im/prefix"),
                        [new SwarmUri(ones, "absolute/path")],
                        new SwarmAddress(ones, "absolute/path")),

                    // Multiple mixed uris.
                    new(new SwarmAddress(SwarmReference.PlainZero),
                        ["a", "b/c", new SwarmUri(ones, null), "d"],
                        new SwarmAddress(ones, "d")),
                };

                return tests.Select(t => new object[] { t });
            }
        }

        public static IEnumerable<object[]> StringToAddressTests
        {
            get
            {
                var tests = new List<StringToAddressTestElement>
                {
                    // Only hash without ending slash.
                    new("0000000000000000000000000000000000000000000000000000000000000000",
                        SwarmReference.PlainZero,
                        "/"),
                    
                    // Only hash with ending slash.
                    new("0000000000000000000000000000000000000000000000000000000000000000/",
                        SwarmReference.PlainZero,
                        "/"),
                    
                    // With initial root.
                    new("/0000000000000000000000000000000000000000000000000000000000000000",
                        SwarmReference.PlainZero,
                        "/"),
                    
                    // With initial root and ending slash.
                    new("/0000000000000000000000000000000000000000000000000000000000000000/",
                        SwarmReference.PlainZero,
                        "/"),
                    
                    // With path.
                    new("0000000000000000000000000000000000000000000000000000000000000000/Im/a/path",
                        SwarmReference.PlainZero,
                        "/Im/a/path"),
                    
                    // With initial root and path.
                    new("/0000000000000000000000000000000000000000000000000000000000000000/Im/a/path",
                        SwarmReference.PlainZero,
                        "/Im/a/path"),
                    
                    // With final slash.
                    new("0000000000000000000000000000000000000000000000000000000000000000/I/have/final/slash/",
                        SwarmReference.PlainZero,
                        "/I/have/final/slash/"),
                    
                    // With special chars.
                    new("0000000000000000000000000000000000000000000000000000000000000000/I have a % of special\\chars!",
                        SwarmReference.PlainZero,
                        "/I have a % of special\\chars!")
                };

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

        [Theory, MemberData(nameof(CombineUrisTests))]
        public void CombineUris(CombineUrisTestElement test)
        {
            var result = test.Address.Combine(test.InputUris);

            Assert.Equal(test.ExpectedAddress.Reference, result.Reference);
            Assert.Equal(test.ExpectedAddress.Path, result.Path);
        }

        [Theory, MemberData(nameof(StringToAddressTests))]
        public void StringToAddress(StringToAddressTestElement test)
        {
            var result = new SwarmAddress(test.InputString);
            
            Assert.Equal(test.ExpectedReference, result.Reference);
            Assert.Equal(test.ExpectedRelativePath, result.Path);
        }
    }
}