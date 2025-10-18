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

using Etherna.BeeNet.Models;

namespace Etherna.BeeNet.Extensions
{
    public static class RedundancyLevelExtensions
    {
        /// <summary>
        /// Returns the maximum number of effective data chunks
        /// </summary>
        /// <param name="level">Redundancy level</param>
        /// <returns>Maximum number of effective data chunks</returns>
        public static int GetMaxShards(this RedundancyLevel level, bool isEncrypted)
        {
            var parities = level.GetParities(
                isEncrypted,
                isEncrypted ?
                    SwarmChunkBmt.EncryptedSegmentsCount :
                    SwarmChunkBmt.SegmentsCount);
            return isEncrypted ?
                (SwarmChunkBmt.SegmentsCount - parities) / 2 :
                SwarmChunkBmt.SegmentsCount - parities;
        }
        
        public static int GetParities(this RedundancyLevel level, bool isEncrypted, int shards)
        {
            var erasureTable = ErasureTable.TryGetFromRedundancyLevel(level, isEncrypted);
            return erasureTable?.GetOptimalParities(shards) ?? 0;
        }
    }
}