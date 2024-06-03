// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Hasher.Bmt;
using Etherna.BeeNet.Hasher.Redundancy;
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
                _ => throw new InvalidOperationException($"redundancy: level value {level} is not a legit redundancy level")
            };
        
        public static ErasureTable? TryGetErasureTable(this RedundancyLevel level) =>
            level switch
            {
                RedundancyLevel.None => null,
                RedundancyLevel.Medium => ErasureTable.Medium,
                RedundancyLevel.Strong => ErasureTable.Strong,
                RedundancyLevel.Insane => ErasureTable.Insane,
                RedundancyLevel.Paranoid => ErasureTable.Paranoid,
                _ => throw new InvalidOperationException($"redundancy: level value {level} is not a legit redundancy level")
            };

        public static int GetEncryptedParities(this RedundancyLevel level, int shards)
        {
            var erasureTable = level.TryGetEncryptedErasureTable();
            return erasureTable?.GetOptimalParities(shards) ?? 0;
        }
        
        public static int GetMaxEncryptedShards(this RedundancyLevel level)
        {
            var parities = level.GetEncryptedParities(RedundancyParams.EncryptedBmtSegments);
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

        public static int GetReplicaCount(this RedundancyLevel level) =>
            level switch
            {
                RedundancyLevel.None => 0,
                RedundancyLevel.Medium => 2,
                RedundancyLevel.Strong => 4,
                RedundancyLevel.Insane => 8,
                RedundancyLevel.Paranoid => 16,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
    }
}