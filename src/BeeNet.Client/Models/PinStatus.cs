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
    public sealed class PinStatus(
        SwarmReference reference,
        DateTimeOffset? creationTime,
        IEnumerable<SwarmHash>? missingChunks,
        bool? processed,
        bool succeeded,
        long? pinnedChunks)
    {
        public SwarmReference Reference { get; } = reference;
        public DateTimeOffset? CreationTime { get; } = creationTime;
        public bool? Processed { get; } = processed;
        public bool Succeeded { get; } = succeeded;
        public IEnumerable<SwarmHash>? MissingChunks { get; } = missingChunks;
        public long? PinnedChunks { get; } = pinnedChunks;
    }
}