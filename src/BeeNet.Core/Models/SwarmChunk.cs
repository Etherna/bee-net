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
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Models
{
    public abstract class SwarmChunk
    {
        // Properties.
        public abstract SwarmHash Hash { get; }
        
        // Methods.
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not SwarmChunk objChunk) return false;
            return GetType() == obj.GetType() &&
                   Hash.Equals(objChunk.Hash) &&
                   GetFullPayload().Span.SequenceEqual(objChunk.GetFullPayload().Span);
        }
        public abstract ReadOnlyMemory<byte> GetFullPayload();
        public abstract byte[] GetFullPayloadToByteArray();
        public override int GetHashCode() =>
            Hash.GetHashCode() ^
            GetFullPayload().GetHashCode();
    }
}