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
using System;

namespace Etherna.BeeNet.Extensions
{
    public static class RedundancyLevelExtensions
    {
        public static ErasureTable? TryGetEncryptedErasureTable(this RedundancyLevel level) =>
            level switch
            {
                RedundancyLevel.None => null,
                RedundancyLevel.Medium => ErasureTable.EncMedium,
                RedundancyLevel.Strong => ErasureTable.EncStrong,
                RedundancyLevel.Insane => ErasureTable.EncInsane,
                RedundancyLevel.Paranoid => ErasureTable.EncParanoid,
                _ => throw new InvalidOperationException()
            };
        
        public static ErasureTable? TryGetErasureTable(this RedundancyLevel level) =>
            level switch
            {
                RedundancyLevel.None => null,
                RedundancyLevel.Medium => ErasureTable.Medium,
                RedundancyLevel.Strong => ErasureTable.Strong,
                RedundancyLevel.Insane => ErasureTable.Insane,
                RedundancyLevel.Paranoid => ErasureTable.Paranoid,
                _ => throw new InvalidOperationException()
            };

        public static int GetEncryptedParities(this RedundancyLevel level, int shards)
        {
            var erasureTable = level.TryGetEncryptedErasureTable();
            return erasureTable?.GetOptimalParities(shards) ?? 0;
        }
        
        public static int GetMaxEncryptedShards(this RedundancyLevel level)
        {
            var parities = level.GetEncryptedParities(SwarmChunkBmt.EncryptedSegmentsCount);
            return (SwarmChunkBmt.SegmentsCount - parities) / 2;
        }
        
        /// <summary>
        /// Returns the maximum number of effective data chunks
        /// </summary>
        /// <param name="level">Redundancy level</param>
        /// <returns>Maximum number of effective data chunks</returns>
        public static int GetMaxShards(this RedundancyLevel level)
        {
            var parities = level.GetParities(SwarmChunkBmt.SegmentsCount);
            return SwarmChunkBmt.SegmentsCount - parities;
        }

        public static int GetParities(this RedundancyLevel level, int shards)
        {
            var erasureTable = level.TryGetErasureTable();
            return erasureTable?.GetOptimalParities(shards) ?? 0;
        }

        /// <summary>
        /// Get the actual number of replicas needed to keep the error rate below 1/10^6.
        /// For the five levels of redundancy are 0, 2, 4, 5, 19, we use an approximation as the successive powers of 2.
        /// </summary>
        public static int GetReplicaCount(this RedundancyLevel level) =>
            level switch
            {
                RedundancyLevel.None => 0,
                RedundancyLevel.Medium => 2,
                RedundancyLevel.Strong => 4,
                RedundancyLevel.Insane => 8,
                RedundancyLevel.Paranoid => 16,
                _ => throw new InvalidOperationException()
            };
    }
}