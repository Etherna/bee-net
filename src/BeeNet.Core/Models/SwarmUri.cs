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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    public readonly struct SwarmUri : IEquatable<SwarmUri>
    {
        // Constructor.
        public SwarmUri(SwarmHash? hash, string? path)
        {
            if (hash is null && path is null)
                throw new ArgumentException("Hash and path can't be both null");
            
            Hash = hash;
            Path = hash != null ? SwarmAddress.NormalizePath(path) : path!;
        }
        public SwarmUri(string uri, bool isAbsolute)
        {
            ArgumentNullException.ThrowIfNull(uri, nameof(uri));
            
            if (isAbsolute)
            {
                var address = new SwarmAddress(uri);
                Hash = address.Hash;
                Path = address.Path;
            }
            else
            {
                Path = uri;
            }
        }
        
        // Properties.
        public SwarmHash? Hash { get; }
        public bool IsAbsolute => Hash.HasValue;
        public bool IsRooted => IsAbsolute || System.IO.Path.IsPathRooted(Path);
        public string Path { get; }
        
        // Methods.
        public bool Equals(SwarmUri other) =>
            Hash.Equals(other.Hash) &&
            EqualityComparer<string>.Default.Equals(Path, other.Path);
        
        public override bool Equals(object? obj) => obj is SwarmUri other && Equals(other);
        
        public override int GetHashCode() => Hash.GetHashCode() ^
                                             (Path?.GetHashCode(StringComparison.InvariantCulture) ?? 0);
        
        public override string ToString() =>
            IsAbsolute ? new SwarmAddress(Hash!.Value, Path).ToString() : Path!;
        
        public SwarmAddress ToSwarmAddress(SwarmAddress prefix)
        {
            var combined = Combine(prefix, this);
            return new(combined.Hash!.Value, combined.Path);
        }

        public bool TryGetRelativeTo(SwarmUri relativeTo, out SwarmUri output)
        {
            output = default;
            
            if (relativeTo.Hash != Hash)
                return false;

            if (!Path.StartsWith(relativeTo.Path, StringComparison.InvariantCulture))
                return false;

            output = new SwarmUri(null, Path[relativeTo.Path.Length..].TrimStart('/'));
            return true;
        }
        
        // Static methods.
        public static SwarmUri Combine(params SwarmUri[] uris)
        {
            ArgumentNullException.ThrowIfNull(uris, nameof(uris));
            if (uris.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(uris), "Empty uri list");

            var combined = uris[0];
            foreach (var uri in uris.Skip(1))
            {
                if (uri.IsAbsolute)
                    combined = uri;
                else if (uri.IsRooted)
                    combined = new SwarmUri(combined.Hash, uri.Path);
                else
                    combined = new SwarmUri(combined.Hash, string.Concat(combined.Path ?? "", uri.Path!));
            }

            return combined;
        }
        public static SwarmUri FromSwarmAddress(SwarmAddress value) => new(value.Hash, value.Path);
        public static SwarmUri FromSwarmHash(SwarmHash value) => new(value, null);
        
        // Operator methods.
        public static bool operator ==(SwarmUri left, SwarmUri right) => left.Equals(right);
        public static bool operator !=(SwarmUri left, SwarmUri right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmUri(SwarmAddress value) => new(value.Hash, value.Path);
        public static implicit operator SwarmUri(SwarmHash value) => new(value, null);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmUri value) => value.ToString();
    }
}