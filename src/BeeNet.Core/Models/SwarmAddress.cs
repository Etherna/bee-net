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

namespace Etherna.BeeNet.Models
{
    public readonly struct SwarmAddress : IEquatable<SwarmAddress>
    {
        // Constructor.
        public SwarmAddress(SwarmHash hash, string? path = null)
        {
            Hash = hash;
            Path = path;
        }
        public SwarmAddress(string address)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            
            // Trim initial and final slash.
            address = address.Trim('/');

            // Extract hash root.
            var slashIndex = address.IndexOf('/', StringComparison.InvariantCulture);
            var root = slashIndex > 0 ? address[..slashIndex] : address;
            var path = slashIndex > 0 ? address[(slashIndex + 1)..] : null;
            
            // Set hash and path.
            Hash = new SwarmHash(root);
            if (!string.IsNullOrEmpty(path))
                Path = path;
        }
        
        // Properties.
        public SwarmHash Hash { get; }
        public string? Path { get; }
        
        // Methods.
        public bool Equals(SwarmAddress other) =>
            Hash.Equals(other.Hash) &&
            EqualityComparer<string>.Default.Equals(Path, other.Path);
        public override bool Equals(object? obj) => obj is SwarmAddress other && Equals(other);
        public override int GetHashCode() => Hash.GetHashCode() ^
                                             (Path?.GetHashCode(StringComparison.InvariantCulture) ?? 0);
        public override string ToString()
        {
            if (Path is null)
                return Hash + "/";
            
            if (System.IO.Path.IsPathRooted(Path))
                return Hash + Path;

            return Hash + "/" + Path;
        }
        
        // Static methods.
        public static SwarmAddress FromSwarmHash(SwarmHash value) => new(value);
        public static SwarmAddress FromString(string value) => new(value);
        
        // Operator methods.
        public static bool operator ==(SwarmAddress left, SwarmAddress right) => left.Equals(right);
        public static bool operator !=(SwarmAddress left, SwarmAddress right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmAddress(SwarmHash value) => new(value);
        public static implicit operator SwarmAddress(string value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmAddress value) => value.ToString();
    }
}