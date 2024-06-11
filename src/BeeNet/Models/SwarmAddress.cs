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
using System.IO;

namespace Etherna.BeeNet.Models
{
    public readonly struct SwarmAddress : IEquatable<SwarmAddress>
    {
        // Constructor.
        public SwarmAddress(SwarmHash hash, Uri? relativePath = null)
        {
            if (relativePath is not null &&
                relativePath.IsAbsoluteUri)
                throw new ArgumentException("Path needs to be relative", nameof(relativePath));
                
            Hash = hash;
            RelativePath = relativePath;
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
                RelativePath = new Uri(path, UriKind.Relative);
        }
        
        // Properties.
        public SwarmHash Hash { get; }
        public Uri? RelativePath { get; }
        
        // Methods.
        public bool Equals(SwarmAddress other) =>
            Hash.Equals(other.Hash) &&
            EqualityComparer<Uri>.Default.Equals(RelativePath, other.RelativePath);
        public override bool Equals(object? obj) => obj is SwarmAddress other && Equals(other);
        public override int GetHashCode() => Hash.GetHashCode() ^
                                             (RelativePath?.GetHashCode() ?? 0);
        public override string ToString()
        {
            if (RelativePath is null)
                return Hash + "/";
            
            var pathString = RelativePath.ToString();
            if (Path.IsPathRooted(pathString))
                return Hash + pathString;

            return Hash + "/" + pathString;
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