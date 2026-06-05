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

using Etherna.SwarmSdk.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.SwarmSdk.Models
{
    [TypeConverter(typeof(SwarmAddressTypeConverter))]
    public readonly struct SwarmAddress : IEquatable<SwarmAddress>, IParsable<SwarmAddress>
    {
        // Consts.
        public const char Separator = '/';
        
        // Fields.
        private readonly string? _path;
        
        // Constructor.
        public SwarmAddress(SwarmReference reference, string? path = null)
        {
            Reference = reference;
            _path = NormalizePath(path);
        }
        public SwarmAddress(string address)
        {
            ArgumentNullException.ThrowIfNull(address);
            
            // Trim initial slash.
            address = address.TrimStart(Separator);

            // Extract root reference.
            var slashIndex = address.IndexOf(Separator, StringComparison.InvariantCulture);
            var reference = slashIndex > 0 ? address[..slashIndex] : address;
            var path = slashIndex > 0 ? address[slashIndex..] : Separator.ToString();
            
            // Set reference and path.
            Reference = new SwarmReference(reference);
            _path = NormalizePath(path);
        }
        
        // Properties.
        public SwarmReference Reference { get; }
        public bool HasPath => Path != Separator.ToString();
        public string Path => _path ?? NormalizePath(null);
        
        // Methods.
        /// <summary>
        /// Resolve a sequence of URIs on top of the current Address, and return the resulting Address.
        /// Absolute URIs (with a reference) reset the resolution, while relative URIs append their path.
        /// </summary>
        /// <param name="uris">The URIs to resolve on top of this Address</param>
        /// <returns>The resolved Address</returns>
        public SwarmAddress Combine(params SwarmUri[] uris)
        {
            ArgumentNullException.ThrowIfNull(uris);
            return SwarmUri.Combine([this, .. uris]).ToSwarmAddress();
        }
        public bool Equals(SwarmAddress other) =>
            Reference.Equals(other.Reference) &&
            EqualityComparer<string>.Default.Equals(Path, other.Path);
        public override bool Equals(object? obj) => obj is SwarmAddress other && Equals(other);
        public override int GetHashCode() => Reference.GetHashCode() ^
                                             Path.GetHashCode(StringComparison.InvariantCulture);
        public override string ToString() => Reference + Path;

        // Static methods.
        public static SwarmAddress FromString(string value) => new(value);
        public static SwarmAddress FromSwarmHash(SwarmHash value) => new(value);
        public static SwarmAddress FromSwarmReference(SwarmReference value) => new(value);
        public static SwarmAddress Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SwarmAddress result)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                result = default;
                return false;
            }

#pragma warning disable CA1031
            try
            {
                result = FromString(s);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
#pragma warning restore CA1031
        }
        
        // Operator methods.
        public static bool operator ==(SwarmAddress left, SwarmAddress right) => left.Equals(right);
        public static bool operator !=(SwarmAddress left, SwarmAddress right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmAddress(string value) => new(value);
        public static implicit operator SwarmAddress(SwarmHash value) => new(value);
        public static implicit operator SwarmAddress(SwarmReference value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmAddress value) => value.ToString();
        
        // Helpers.
        internal static string NormalizePath(string? path) =>
            Separator + (path ?? "").TrimStart(Separator);
    }
}