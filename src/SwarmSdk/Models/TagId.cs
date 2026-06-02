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
using System.ComponentModel;
using System.Globalization;

namespace Etherna.SwarmSdk.Models
{
    [TypeConverter(typeof(TagIdTypeConverter))]
    public readonly struct TagId(ulong value) : IEquatable<TagId>
    {
        // Properties.
        public ulong Value { get; } = value;

        // Methods.
        public bool Equals(TagId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is TagId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

        // Operator methods.
        public static bool operator ==(TagId left, TagId right) => left.Equals(right);
        public static bool operator !=(TagId left, TagId right) => !(left == right);
    }
}