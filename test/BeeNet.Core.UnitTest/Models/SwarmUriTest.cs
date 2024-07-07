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
    public class SwarmUriTest
    {
        // Internal classes.
        public class HashAndPathToUriTestElement(
            SwarmHash? inputHash,
            string? inputPath,
            Type? expectedExceptionType,
            SwarmHash? expectedHash,
            string expectedPath,
            UriKind expectedUriKind,
            bool expectedIsRooted)
        {
            public SwarmHash? InputHash { get; } = inputHash;
            public string? InputPath { get; } = inputPath;
            public Type? ExpectedExceptionType { get; } = expectedExceptionType;
            public SwarmHash? ExpectedHash { get; } = expectedHash;
            public string ExpectedPath { get; } = expectedPath;
            public bool ExpectedIsRooted { get; } = expectedIsRooted;
            public UriKind ExpectedUriKind { get; } = expectedUriKind;
        }

        public class StringToUriTestElement(
            string inputString,
            UriKind inputUriKind,
            Type? expectedExceptionType,
            SwarmHash? expectedHash,
            string expectedPath,
            UriKind expectedUriKind,
            bool expectedIsRooted)
        {
            public string InputString { get; } = inputString;
            public UriKind InputUriKind { get; } = inputUriKind;
            public Type? ExpectedExceptionType { get; } = expectedExceptionType;
            public SwarmHash? ExpectedHash { get; } = expectedHash;
            public string ExpectedPath { get; } = expectedPath;
            public bool ExpectedIsRooted { get; } = expectedIsRooted;
            public UriKind ExpectedUriKind { get; } = expectedUriKind;
        }

        public class UriToStringTestElement(
            SwarmUri uri,
            string expectedString)
        {
            public SwarmUri Uri { get; } = uri;
            public string ExpectedString { get; } = expectedString;
        }

        // Data.
        public static IEnumerable<object[]> HashAndPathToUriTests
        {
            get
            {
                var tests = new List<HashAndPathToUriTestElement>
                {
                    // No hash and no path (throws)
                    new(null,
                        null,
                        typeof(ArgumentException),
                        null,
                        "",
                        UriKind.RelativeOrAbsolute,
                        false),
                    
                    // Only hash.
                    new(SwarmHash.Zero,
                        null,
                        null,
                        SwarmHash.Zero,
                        "/",
                        UriKind.Absolute,
                        true),
                    
                    // No hash and not rooted path.
                    new(null,
                        "not/rooted/path",
                        null,
                        null,
                        "not/rooted/path",
                        UriKind.Relative,
                        false),
                    
                    // No hash and rooted path.
                    new(null,
                        "/rooted/path",
                        null,
                        null,
                        "/rooted/path",
                        UriKind.Relative,
                        true),
                    
                    // Hash and not rooted path.
                    new(SwarmHash.Zero,
                        "not/rooted/path",
                        null,
                        SwarmHash.Zero,
                        "/not/rooted/path",
                        UriKind.Absolute,
                        true),
                    
                    // Hash and rooted path.
                    new(SwarmHash.Zero,
                        "/rooted/path",
                        null,
                        SwarmHash.Zero,
                        "/rooted/path",
                        UriKind.Absolute,
                        true),
                };

                return tests.Select(t => new object[] { t });
            }
        }
        
        public static IEnumerable<object[]> StringToUriTests
        {
            get
            {
                var tests = new List<StringToUriTestElement>
                {
                    // RelativeOrAbsolute, not rooted, not starting with hash.
                    new("not/rooted/path",
                        UriKind.RelativeOrAbsolute,
                        null,
                        null,
                        "not/rooted/path",
                        UriKind.Relative,
                        false),
                    
                    // RelativeOrAbsolute, rooted, not starting with hash.
                    new("/rooted/path",
                        UriKind.RelativeOrAbsolute,
                        null,
                        null,
                        "/rooted/path",
                        UriKind.Relative,
                        true),
                    
                    // RelativeOrAbsolute, only hash.
                    new("0000000000000000000000000000000000000000000000000000000000000000",
                        UriKind.RelativeOrAbsolute,
                        null,
                        SwarmHash.Zero,
                        "/",
                        UriKind.Absolute,
                        true),
                    
                    // RelativeOrAbsolute, not rooted, starting with hash.
                    new("0000000000000000000000000000000000000000000000000000000000000000/not/rooted/path",
                        UriKind.RelativeOrAbsolute,
                        null,
                        SwarmHash.Zero,
                        "/not/rooted/path",
                        UriKind.Absolute,
                        true),
                    
                    // RelativeOrAbsolute, rooted, starting with hash.
                    new("/0000000000000000000000000000000000000000000000000000000000000000/rooted/path",
                        UriKind.RelativeOrAbsolute,
                        null,
                        null,
                        "/0000000000000000000000000000000000000000000000000000000000000000/rooted/path",
                        UriKind.Relative,
                        true),
                    
                    // Relative not rooted path.
                    new("relative/not/rooted/path",
                        UriKind.Relative,
                        null,
                        null,
                        "relative/not/rooted/path",
                        UriKind.Relative,
                        false),
                    
                    // Relative rooted path.
                    new("/relative/rooted/path",
                        UriKind.Relative,
                        null,
                        null,
                        "/relative/rooted/path",
                        UriKind.Relative,
                        true),
                    
                    // Absolute with only hash (without slashes).
                    new("0000000000000000000000000000000000000000000000000000000000000000",
                        UriKind.Absolute,
                        null,
                        SwarmHash.Zero,
                        "/",
                        UriKind.Absolute,
                        true),
                    
                    // Absolute with only hash (with slashes).
                    new("/0000000000000000000000000000000000000000000000000000000000000000/",
                        UriKind.Absolute,
                        null,
                        SwarmHash.Zero,
                        "/",
                        UriKind.Absolute,
                        true),
                    
                    // Absolute with hash and path.
                    new("0000000000000000000000000000000000000000000000000000000000000000/Im/a/path",
                        UriKind.Absolute,
                        null,
                        SwarmHash.Zero,
                        "/Im/a/path",
                        UriKind.Absolute,
                        true),
                    
                    // Absolute with invalid initial hash (throws).
                    new("not/An/Hash",
                        UriKind.Absolute,
                        typeof(ArgumentException),
                        null,
                        "",
                        UriKind.RelativeOrAbsolute,
                        false)
                };

                return tests.Select(t => new object[] { t });
            }
        }
        
        public static IEnumerable<object[]> UriToStringTests
        {
            get
            {
                var tests = new List<UriToStringTestElement>
                {
                    // Relative with not rooted path.
                    new(new SwarmUri(null, "relative/not/rooted/path"),
                        "relative/not/rooted/path"),
                    
                    // Relative with rooted path.
                    new(new SwarmUri(null, "/relative/rooted/path"),
                        "/relative/rooted/path"),
                    
                    // Absolute with not rooted path.
                    new(new SwarmUri(SwarmHash.Zero, "relative/not/rooted/path"),
                        "0000000000000000000000000000000000000000000000000000000000000000/relative/not/rooted/path"),
                    
                    // Absolute with rooted path.
                    new(new SwarmUri(SwarmHash.Zero, "/relative/rooted/path"),
                        "0000000000000000000000000000000000000000000000000000000000000000/relative/rooted/path"),
                };

                return tests.Select(t => new object[] { t });
            }
        }
        
        // Tests.
        [Theory, MemberData(nameof(HashAndPathToUriTests))]
        public void HashAndPathToUri(HashAndPathToUriTestElement test)
        {
            if (test.ExpectedExceptionType is not null)
            {
                Assert.Throws(
                    test.ExpectedExceptionType,
                    () => new SwarmUri(test.InputHash, test.InputPath));
            }
            else
            {
                var result = new SwarmUri(test.InputHash, test.InputPath);
        
                Assert.Equal(test.ExpectedHash, result.Hash);
                Assert.Equal(test.ExpectedPath, result.Path);
                Assert.Equal(test.ExpectedUriKind, result.UriKind);
                Assert.Equal(test.ExpectedIsRooted, result.IsRooted);
            }
        }
        
        [Theory, MemberData(nameof(StringToUriTests))]
        public void StringToUri(StringToUriTestElement test)
        {
            if (test.ExpectedExceptionType is not null)
            {
                Assert.Throws(
                    test.ExpectedExceptionType,
                    () => new SwarmUri(test.InputString, test.InputUriKind));
            }
            else
            {
                var result = new SwarmUri(test.InputString, test.InputUriKind);

                Assert.Equal(test.ExpectedHash, result.Hash);
                Assert.Equal(test.ExpectedPath, result.Path);
                Assert.Equal(test.ExpectedUriKind, result.UriKind);
                Assert.Equal(test.ExpectedIsRooted, result.IsRooted);
            }
        }
        
        [Theory, MemberData(nameof(UriToStringTests))]
        public void UriToString(UriToStringTestElement test)
        {
            var result = test.Uri.ToString();
            
            Assert.Equal(test.ExpectedString, result);
        }
    }
}