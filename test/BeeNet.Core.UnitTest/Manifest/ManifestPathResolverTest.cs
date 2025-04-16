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

using Etherna.BeeNet.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Manifest
{
    public class ManifestPathResolverTest
    {
        // Internal classes.
        public class InvokeResolvingPathTestElement
        {
            public required string Path { get; init; }
            public required ManifestPathResolver Resolver { get; init; }
            public Dictionary<string, string> RootMetadata { get; init; } = new()
            {
                [ManifestEntry.WebsiteIndexDocPathKey] = "index.html",
                [ManifestEntry.WebsiteErrorDocPathKey] = "error.html"
            };
            public bool ExpectedGetMetadata { get; init; }
            public string[] ExpectedInvokes { get; init; } = [];
            public string[] ExpectedPrefixChecked { get; init; } = [];
            public string? ExpectedResult { get; init; }
            public bool ExpectedKeyNotFoundException { get; init; }
            public string? ExpectedRedirectExceptionPath {get; init; }
        }

        // Data.
        public static IEnumerable<object[]> InvokeResolvingPathTests
        {
            get
            {
                var tests = new List<InvokeResolvingPathTestElement>
                {
                    // Don't trim start separator if disabled redirect to directory.
                    new()
                    {
                        Path = "///index.html",
                        Resolver = new ManifestPathResolver(redirectToDirectory: false),
                        ExpectedInvokes = ["///index.html"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Can trim start separators.
                    new()
                    {
                        Path = "///index.html",
                        Resolver = new ManifestPathResolver(redirectToDirectory: true),
                        ExpectedInvokes = ["index.html"],
                        ExpectedResult = "Content on index.html"
                    },
                    
                    // Can trim start separators, even with explicit directory redirect.
                    new()
                    {
                        Path = "///index.html",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: true),
                        ExpectedInvokes = ["index.html"],
                        ExpectedResult = "Content on index.html"
                    },
                    
                    // Throw KeyNotFoundException on empty path, without directory redirect.
                    new()
                    {
                        Path = "",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: false),
                        ExpectedInvokes = [""],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw explicit redirect to root on empty path, with explicit directory redirect.
                    new()
                    {
                        Path = "",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: true),
                        ExpectedRedirectExceptionPath = "/"
                    },
                    
                    // Can resolve index document on empty path, with implicit directory redirect and metadata resolution.
                    new()
                    {
                        Path = "",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["/", "index.html"],
                        ExpectedResult = "Content on index.html"
                    },
                    
                    // Throw KeyNotFoundException on empty path, with implicit directory redirect and no set metadata.
                    new()
                    {
                        Path = "",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        RootMetadata = [],
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on empty path, with implicit directory redirect and no metadata resolution.
                    new()
                    {
                        Path = "",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: false,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedInvokes = ["/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on root with no metadata resolution.
                    new()
                    {
                        Path = "/",
                        Resolver = new ManifestPathResolver(resolveMetadataDocuments: false),
                        ExpectedInvokes = ["/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Can resolve index document on root with metadata resolution.
                    new()
                    {
                        Path = "/",
                        Resolver = new ManifestPathResolver(resolveMetadataDocuments: true),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["/", "index.html"],
                        ExpectedResult = "Content on index.html"
                    },
                    
                    // Can serve content.
                    new()
                    {
                        Path = "index.html",
                        Resolver = new ManifestPathResolver(),
                        ExpectedInvokes = ["index.html"],
                        ExpectedResult = "Content on index.html"
                    },
                    
                    // Can serve content with initial slash, and implicit redirect to directory.
                    new()
                    {
                        Path = "/index.html",
                        Resolver = new ManifestPathResolver(redirectToDirectory: true),
                        ExpectedInvokes = ["index.html"],
                        ExpectedResult = "Content on index.html"
                    },
                    
                    // Can serve content with initial slash, and explicit redirect to directory.
                    new()
                    {
                        Path = "/index.html",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: true),
                        ExpectedInvokes = ["index.html"],
                        ExpectedResult = "Content on index.html"
                    },
                    
                    // Throw KeyNotFoundException with initial slash, and no redirect to directory.
                    new()
                    {
                        Path = "/index.html",
                        Resolver = new ManifestPathResolver(),
                        ExpectedInvokes = ["/index.html"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Can't serve content with wrong final slash, throw KeyNotFoundException without metadata resolution.
                    new()
                    {
                        Path = "index.html/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: false),
                        ExpectedInvokes = ["index.html/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Can't serve content with wrong final slash, return error document with metadata resolution.
                    new()
                    {
                        Path = "index.html/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["index.html/", "index.html/index.html", "error.html"],
                        ExpectedResult = "Content on error.html"
                    },
                    
                    // Throw KeyNotFoundException on directory name, without directory redirect.
                    new()
                    {
                        Path = "indexedDir",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: false),
                        ExpectedInvokes = ["indexedDir"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw explicit redirect on directory name, with explicit directory redirect.
                    new()
                    {
                        Path = "indexedDir",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: true),
                        ExpectedInvokes = ["indexedDir"],
                        ExpectedPrefixChecked = ["indexedDir/"],
                        ExpectedRedirectExceptionPath = "indexedDir/"
                    },
                    
                    // Can resolve index document into directory name, with implicit directory redirect and metadata resolution.
                    new()
                    {
                        Path = "indexedDir",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["indexedDir", "indexedDir/index.html"],
                        ExpectedPrefixChecked = ["indexedDir/"],
                        ExpectedResult = "Content on indexedDir/index.html"
                    },
                    
                    // Throw KeyNotFoundException on directory name, with implicit directory redirect and no set metadata.
                    new()
                    {
                        Path = "indexedDir",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        RootMetadata = [],
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["indexedDir", "indexedDir/"],
                        ExpectedPrefixChecked = ["indexedDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on directory name, with implicit directory redirect and no metadata resolution.
                    new()
                    {
                        Path = "indexedDir",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: false,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedInvokes = ["indexedDir", "indexedDir/"],
                        ExpectedPrefixChecked = ["indexedDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on directory, without directory redirect.
                    new()
                    {
                        Path = "indexedDir/",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: false),
                        ExpectedInvokes = ["indexedDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on directory, with explicit directory redirect.
                    new()
                    {
                        Path = "indexedDir/",
                        Resolver = new ManifestPathResolver(
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: true),
                        ExpectedInvokes = ["indexedDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Can resolve index document into directory, with implicit directory redirect and metadata resolution.
                    new()
                    {
                        Path = "indexedDir/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["indexedDir/", "indexedDir/index.html"],
                        ExpectedResult = "Content on indexedDir/index.html"
                    },
                    
                    // Throw KeyNotFoundException on directory, with implicit directory redirect and no set metadata.
                    new()
                    {
                        Path = "indexedDir/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        RootMetadata = [],
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["indexedDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on directory, with implicit directory redirect and no metadata resolution.
                    new()
                    {
                        Path = "indexedDir/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: false,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedInvokes = ["indexedDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Resolve error document on directory name, not containing an index, with implicit directory redirect and metadata resolution
                    new()
                    {
                        Path = "noIndexDir",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["noIndexDir", "noIndexDir/index.html", "error.html"],
                        ExpectedPrefixChecked = ["noIndexDir/"],
                        ExpectedResult = "Content on error.html"
                    },
                    
                    // Resolve error document into directory, with implicit directory redirect and metadata resolution.
                    new()
                    {
                        Path = "noIndexDir/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true,
                            explicitRedirectToDirectory: false),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["noIndexDir/", "noIndexDir/index.html", "error.html"],
                        ExpectedResult = "Content on error.html"
                    },
                    
                    // Resolve error document on not existing file, with metadata resolution
                    new()
                    {
                        Path = "notFile",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["notFile", "error.html"],
                        ExpectedPrefixChecked = ["notFile/"],
                        ExpectedResult = "Content on error.html"
                    },
                    
                    // Throw KeyNotFoundException on not existing file, without metadata resolution
                    new()
                    {
                        Path = "notFile",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: false,
                            redirectToDirectory: true),
                        ExpectedInvokes = ["notFile"],
                        ExpectedPrefixChecked = ["notFile/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on not existing file, with no set metadata
                    new()
                    {
                        Path = "notFile",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true),
                        RootMetadata = [],
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["notFile"],
                        ExpectedPrefixChecked = ["notFile/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Resolve error document on not existing directory, with metadata resolution
                    new()
                    {
                        Path = "notDir/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true),
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["notDir/", "notDir/index.html", "error.html"],
                        ExpectedResult = "Content on error.html"
                    },
                    
                    // Throw KeyNotFoundException on not existing directory, without metadata resolution
                    new()
                    {
                        Path = "notDir/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: false,
                            redirectToDirectory: true),
                        ExpectedInvokes = ["notDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                    
                    // Throw KeyNotFoundException on not existing directory, with no set metadata
                    new()
                    {
                        Path = "notDir/",
                        Resolver = new ManifestPathResolver(
                            resolveMetadataDocuments: true,
                            redirectToDirectory: true),
                        RootMetadata = [],
                        ExpectedGetMetadata = true,
                        ExpectedInvokes = ["notDir/"],
                        ExpectedKeyNotFoundException = true
                    },
                };

                return tests.Select(t => new object[] { t });
            }
        }
        
        // Tests.
        [Theory, MemberData(nameof(InvokeResolvingPathTests))]
        public async Task InvokeResolvingPath(InvokeResolvingPathTestElement test)
        {
            // Setup.
            var invokeResults = new Dictionary<string, string>
            {
                ["error.html"] =            "Content on error.html",
                ["index.html"] =            "Content on index.html",
                ["indexedDir/index.html"] = "Content on indexedDir/index.html",
                ["noIndexDir/text.txt"] =   "Content on noIndexDir/text.txt",
            };
            
            List<string> invokedOnPaths = new();
            List<string> prefixCheckedOnPaths = new();
            int getRootMetadataCounter = 0;

            // Action and asserts.
            Task<string> InvokeFunc() =>
                test.Resolver.InvokeAsync(
                    test.Path,
                    invokeAsync: p =>
                    {
                        invokedOnPaths.Add(p);
                        if (invokeResults.TryGetValue(p, out var invokeResult))
                            return Task.FromResult(invokeResult);
                        throw new KeyNotFoundException();
                    },
                    hasPathPrefixAsync: p =>
                    {
                        prefixCheckedOnPaths.Add(p);
                        return Task.FromResult(invokeResults.Keys.Any(k => k.StartsWith(p)));
                    },
                    getRootMetadataAsync: () =>
                    {
                        getRootMetadataCounter++;
                        return Task.FromResult<IReadOnlyDictionary<string, string>>(test.RootMetadata);
                    });

            if (test.ExpectedResult != null)
            {
                var result = await InvokeFunc();

                Assert.Equal(test.ExpectedResult, result);
            }
            else
            {
                if (test.ExpectedKeyNotFoundException)
                    await Assert.ThrowsAsync<KeyNotFoundException>(async () => await InvokeFunc());
                else
                {
                    try
                    {
                        await InvokeFunc();
                        Assert.Fail("Expected ManifestExplicitRedirectException was not thrown.");
                    }
                    catch (ManifestExplicitRedirectException e)
                    {
                        Assert.Equal(test.ExpectedRedirectExceptionPath!, e.RedirectToPath);
                    }
                }
            }
            
            Assert.Equal(test.ExpectedInvokes, invokedOnPaths);
            Assert.Equal(test.ExpectedPrefixChecked, prefixCheckedOnPaths);
            Assert.Equal(test.ExpectedGetMetadata ? 1 : 0, getRootMetadataCounter);
        }
    }
}