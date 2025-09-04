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
using System.Threading.Tasks;

namespace Etherna.BeeNet.Manifest
{
    public abstract class MantarayNodeBase : IReadOnlyMantarayNode
    {
        // Consts.
        public const int ForksIndexSize = 32;
        public static readonly byte[] Version02Hash = new Hasher().ComputeHash("mantaray:0.2")
            .Take(VersionHashSize).ToArray();
        public const int VersionHashSize = 31;

        // Properties.
        public abstract SwarmReference? EntryReference { get; }
        public abstract IReadOnlyDictionary<char, MantarayNodeFork> Forks { get; }
        public abstract IReadOnlyDictionary<string, string> Metadata { get; }
        public abstract NodeType NodeTypeFlags { get; }
        public abstract EncryptionKey256? ObfuscationKey { get; }
        public abstract SwarmReference Reference { get; }
        
        // Methods.
        public async Task<IReadOnlyDictionary<string, string>> GetMetadataAsync(
            string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            // If the path is empty, return current node metadata
            if (path.Length == 0)
                return Metadata;

            // Find the child fork.
            if (!Forks.TryGetValue(path[0], out var fork) ||
                !path.StartsWith(fork.Prefix, StringComparison.InvariantCulture))
                throw new KeyNotFoundException($"Final path {path} can't be found");
            
            // If the child node is the one we are looking for, return metadata.
            var childSubPath = path[fork.Prefix.Length..];
            if (childSubPath.Length == 0)
                return fork.Node.Metadata;
            
            // Else, proceed into it.
            await fork.Node.OnVisitingAsync().ConfigureAwait(false);

            return await fork.Node.GetMetadataAsync(childSubPath).ConfigureAwait(false);
        }
        
        public async Task<MantarayResourceInfo> GetResourceInfoAsync(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            // If the path is empty and entry is not null, return the entry
            if (path.Length == 0)
            {
                if (EntryReference.HasValue && !SwarmReference.IsZero(EntryReference.Value))
                    return new()
                    {
                        Reference = EntryReference.Value,
                        Metadata = Metadata,
                    };
            
                throw new KeyNotFoundException("Path can't be found");
            }
            
            // Find the child fork.
            if (!Forks.TryGetValue(path[0], out var fork) ||
                !path.StartsWith(fork.Prefix, StringComparison.InvariantCulture))
                throw new KeyNotFoundException($"Final path {path} can't be found");

            await fork.Node.OnVisitingAsync().ConfigureAwait(false);

            return await fork.Node.GetResourceInfoAsync(path[fork.Prefix.Length..]).ConfigureAwait(false);
        }
        
        public async Task<bool> HasPathPrefixAsync(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            if (path.Length == 0)
                return true;
            
            // Find the child fork.
            if (!Forks.TryGetValue(path[0], out var fork))
                return false;
            
            var commonPathLength = Math.Min(path.Length, fork.Prefix.Length);
            if (!path.AsSpan()[..commonPathLength].SequenceEqual(fork.Prefix.AsSpan()[..commonPathLength]))
                return false;

            await fork.Node.OnVisitingAsync().ConfigureAwait(false);

            return await fork.Node.HasPathPrefixAsync(
                path[commonPathLength..]).ConfigureAwait(false);
        }
        
        public abstract Task OnVisitingAsync();
    }
}