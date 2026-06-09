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
    /// Identifies how a feed update payload encodes its wrapped data chunk.
    /// </summary>
    public enum SwarmFeedPayloadVersion
    {
        /// <summary>
        /// Legacy payload: the wrapped chunk embeds a [timestamp][reference] pointing to the
        /// actual data chunk, which has to be resolved with an additional lookup.
        /// </summary>
        V1,

        /// <summary>
        /// Current payload: the wrapped chunk embeds the data chunk directly, no further lookup needed.
        /// </summary>
        V2
    }
}
