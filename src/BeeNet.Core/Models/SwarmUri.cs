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

using Etherna.BeeNet.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    [TypeConverter(typeof(SwarmUriTypeConverter))]
    public readonly struct SwarmUri : IEquatable<SwarmUri>, IParsable<SwarmUri>
    {
        // Fields.
        private readonly string? _path;
        
        // Constructor.
        public SwarmUri(SwarmReference? reference, string? path)
        {
            if (reference is null && path is null)
                throw new ArgumentException("Reference and path can't be both null");
            
            Reference = reference;
            _path = reference != null ? SwarmAddress.NormalizePath(path) : path!;
        }
        public SwarmUri(string uri, UriKind uriKind)
        {
            ArgumentNullException.ThrowIfNull(uri, nameof(uri));
            
            // Determine uri kind.
            if (uriKind == UriKind.RelativeOrAbsolute)
                uriKind = SwarmReference.IsValidReference(uri.Split(SwarmAddress.Separator)[0])
                    ? UriKind.Absolute
                    : UriKind.Relative;
            
            if (uriKind == UriKind.Absolute)
            {
                var address = new SwarmAddress(uri);
                Reference = address.Reference;
                _path = address.Path;
            }
            else
            {
                _path = uri;
            }
        }
        
        // Properties.
        public SwarmReference? Reference { get; }
        public bool HasPath => Path.Length > 0 && Path != SwarmAddress.Separator.ToString();
        public bool IsRooted => UriKind == UriKind.Absolute || System.IO.Path.IsPathRooted(Path);
        public string Path => _path ?? SwarmAddress.NormalizePath(null);
        public UriKind UriKind => Reference.HasValue ? UriKind.Absolute : UriKind.Relative;
        
        // Methods.
        public bool Equals(SwarmUri other) =>
            Reference.Equals(other.Reference) &&
            EqualityComparer<string>.Default.Equals(Path, other.Path);
        
        public override bool Equals(object? obj) => obj is SwarmUri other && Equals(other);
        
        public override int GetHashCode() => Reference.GetHashCode() ^
                                             Path.GetHashCode(StringComparison.InvariantCulture);
        
        public override string ToString() =>
            UriKind == UriKind.Absolute ? new SwarmAddress(Reference!.Value, Path).ToString() : Path;
        
        /// <summary>
        /// Convert a URI to an Address. If URI is relative, a prefix Address must be provided
        /// </summary>
        /// <param name="prefix">Optional prefix address</param>
        /// <returns>The absolute URI as an Address</returns>
        public SwarmAddress ToSwarmAddress(SwarmAddress? prefix = null)
        {
            if (prefix is null)
            {
                if (UriKind != UriKind.Absolute)
                    throw new InvalidOperationException("Url is not absolute, and a prefix address is required");
                
                return new SwarmAddress(Reference!.Value, Path);
            }
            
            var combined = Combine(prefix.Value, this);
            return new(combined.Reference!.Value, combined.Path);
        }

        public bool TryGetRelativeTo(SwarmUri relativeTo, out SwarmUri output)
        {
            output = default;
            
            if (relativeTo.Reference != Reference)
                return false;

            var dirs = Path.Split(SwarmAddress.Separator);
            var relativeToDirs = relativeTo.Path.TrimEnd(SwarmAddress.Separator).Split(SwarmAddress.Separator);
            if (dirs.Length < relativeToDirs.Length ||
                !dirs.AsSpan(0, relativeToDirs.Length).SequenceEqual(relativeToDirs))
                return false;

            output = new SwarmUri(null, string.Join(SwarmAddress.Separator, dirs[relativeToDirs.Length..]));
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
                if (uri.UriKind == UriKind.Absolute)
                    combined = uri;
                else if (uri.IsRooted)
                    combined = new SwarmUri(combined.Reference, uri.Path);
                else
                    combined = new SwarmUri(
                        combined.Reference,
                        combined.Path.TrimEnd(SwarmAddress.Separator) + SwarmAddress.Separator + uri.Path);
            }

            return combined;
        }
        public static SwarmUri FromString(string value) => new(value, UriKind.RelativeOrAbsolute);
        public static SwarmUri FromSwarmAddress(SwarmAddress value) => new(value.Reference, value.Path);
        public static SwarmUri FromSwarmReference(SwarmReference value) => new(value, null);
        public static SwarmUri Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SwarmUri result)
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
        public static bool operator ==(SwarmUri left, SwarmUri right) => left.Equals(right);
        public static bool operator !=(SwarmUri left, SwarmUri right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmUri(string value) => new(value, UriKind.RelativeOrAbsolute);
        public static implicit operator SwarmUri(SwarmAddress value) => new(value.Reference, value.Path);
        public static implicit operator SwarmUri(SwarmReference value) => new(value, null);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmUri value) => value.ToString();
    }
}