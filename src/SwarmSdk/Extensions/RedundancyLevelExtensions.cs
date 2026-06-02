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

using Etherna.SwarmSdk.Models;

namespace Etherna.SwarmSdk.Extensions
{
    public static class RedundancyLevelExtensions
    {
        /// <summary>
        /// Returns the maximum number of effective data references
        /// </summary>
        /// <param name="level">Redundancy level</param>
        /// <returns>Maximum number of effective data references</returns>
        public static int GetMaxDataShards(this RedundancyLevel level, bool isEncrypted)
        {
            var parities = level.GetParitiesAmount(
                isEncrypted,
                isEncrypted ?
                    SwarmChunkBmt.EncryptedSegmentsCount :
                    SwarmChunkBmt.SegmentsCount);
            return isEncrypted ?
                (SwarmChunkBmt.SegmentsCount - parities) / 2 :
                SwarmChunkBmt.SegmentsCount - parities;
        }
        
        public static int GetParitiesAmount(this RedundancyLevel level, bool isEncrypted, int shards)
        {
            var erasureTable = ErasureTable.TryGetFromRedundancyLevel(level, isEncrypted);
            return erasureTable?.GetOptimalParities(shards) ?? 0;
        }
    }
}