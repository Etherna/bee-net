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
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    /// <summary>
    /// Implementation to resolve a path as list of resource lookups.
    /// </summary>
    /// <param name="resolveMetadataDocuments">If true, try to resolve Index and Error documents from metadata</param>
    /// <param name="redirectToDirectory">
    /// Help directories navigation, oriented to browsers:
    /// - Lookup at existing directories with same name when a file is not found, and resolve invocation in it.
    /// - Trim initial separator chars '/' from path into resource lookup. Always implicit.
    /// </param>
    /// <param name="explicitRedirectToDirectory">
    /// If true, throw <see cref="ManifestExplicitRedirectException"/> when a redirect to directory is resolved.
    /// </param>
    public sealed class ManifestPathResolver(
        bool resolveMetadataDocuments = false,
        bool redirectToDirectory = false,
        bool explicitRedirectToDirectory = false)
    {
        // Properties.
        public bool ResolveMetadataDocuments { get; set; } = resolveMetadataDocuments;
        public bool RedirectToDirectory { get; set; } = redirectToDirectory;
        public bool ExplicitRedirectToDirectory { get; set; } = explicitRedirectToDirectory;

        // Static properties.
        public static ManifestPathResolver BrowserResolver { get; } = new(
            resolveMetadataDocuments: true,
            redirectToDirectory: true,
            explicitRedirectToDirectory: true);
        public static ManifestPathResolver IdentityResolver { get; } = new(
            resolveMetadataDocuments: false,
            redirectToDirectory: false,
            explicitRedirectToDirectory: false);
        
        // Methods.
        public async Task<ManifestPathResolutionResult<TResult>> InvokeAsync<TResult>(
            string path,
            Func<string, Task<TResult>> invokeAsync,
            Func<string, Task<bool>> hasPathPrefixAsync,
            Func<Task<IReadOnlyDictionary<string, string>>> getRootMetadataAsync)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            ArgumentNullException.ThrowIfNull(invokeAsync, nameof(invokeAsync));
            ArgumentNullException.ThrowIfNull(hasPathPrefixAsync, nameof(hasPathPrefixAsync));
            ArgumentNullException.ThrowIfNull(getRootMetadataAsync, nameof(getRootMetadataAsync));
                
            // Trim start separators.
            if (RedirectToDirectory)
            {
                var newPath = path.All(c => c == SwarmAddress.Separator)
                    ? SwarmAddress.Separator.ToString()
                    : path.TrimStart(SwarmAddress.Separator);

                // Only explicit redirect when new path is separator, ignore any other start separator trim.
                // This because in general initial trims aren't an issue for browsers, and we can avoid a redirect.
                if (path != newPath &&
                    newPath == SwarmAddress.Separator.ToString() &&
                    ExplicitRedirectToDirectory)
                    throw new ManifestExplicitRedirectException(newPath);
                path = newPath;
            }
            
            IReadOnlyDictionary<string, string>? rootMetadata = null;
            var resolvedIndex = false;
            while (true)
            {
                var retryInvoke = false;
                
                // Invoke with resolutions.
                try
                {
                    var result = await invokeAsync(path).ConfigureAwait(false);
                    return new(result, false);
                }
                catch(KeyNotFoundException)
                {
                    // Check for redirect to existing directory. Example: /mydir -> /mydir/
                    if (RedirectToDirectory &&
                        !resolvedIndex &&
                        !path.EndsWith(SwarmAddress.Separator) &&
                        await hasPathPrefixAsync(path + SwarmAddress.Separator).ConfigureAwait(false))
                    {
                        path += SwarmAddress.Separator;
                        if (ExplicitRedirectToDirectory)
                            throw new ManifestExplicitRedirectException(path);
                      
                        //iterate to allow to add index doc, retry invoke, and then catch error
                        retryInvoke = true;
                    }
                    
                    // Look at metadata.
                    if (ResolveMetadataDocuments)
                    {
                        if (rootMetadata == null)
                            rootMetadata = await getRootMetadataAsync().ConfigureAwait(false);
                        
                        // Check index suffix to path.
                        if (path.EndsWith(SwarmAddress.Separator) &&
                            rootMetadata.TryGetValue(ManifestEntry.WebsiteIndexDocPathKey, out var indexDocument) &&
                            Path.GetFileName(path) != indexDocument)
                        {
                            //allow iteration to catch eventual error page
                            path = path == SwarmAddress.Separator.ToString()
                                ? indexDocument
                                : Path.Combine(path, indexDocument);
                            resolvedIndex = true;
                            retryInvoke = true;
                        }
                
                        // Else, check if error document is to be shown.
                        if (!retryInvoke &&
                            rootMetadata.TryGetValue(ManifestEntry.WebsiteErrorDocPathKey, out var errorDocument) &&
                            path != errorDocument)
                        {
                            //don't iterate on error, to avoid infinite execution vulnerability.  
                            return new(await invokeAsync(errorDocument).ConfigureAwait(false), true);
                        }
                    }

                    if (!retryInvoke)
                        throw;
                }
            }
        }
    }
}