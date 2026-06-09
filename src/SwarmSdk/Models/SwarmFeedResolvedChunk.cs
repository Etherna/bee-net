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

namespace Etherna.SwarmSdk.Models
{
    /// <summary>
    /// The data chunk wrapped by a feed update, together with the payload version it was resolved from.
    /// </summary>
    /// <param name="Chunk">The resolved wrapped data chunk.</param>
    /// <param name="Version">The payload version that produced the chunk.</param>
    public sealed record SwarmFeedResolvedChunk(SwarmCac Chunk, SwarmFeedPayloadVersion Version);
}
