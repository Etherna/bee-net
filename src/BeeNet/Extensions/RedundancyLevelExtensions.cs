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

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Models.Bmt;
using System;

namespace Etherna.BeeNet.Extensions
{
    public static class RedundancyLevelExtensions
    {
        public static ErasureTable GetEncErasureTable(this RedundancyLevel level) =>
            level switch
            {
                RedundancyLevel.None => throw new InvalidOperationException("redundancy: level NONE does not have erasure table"),
                RedundancyLevel.Medium => ErasureTable.EncMedium,
                RedundancyLevel.Strong => ErasureTable.EncStrong,
                RedundancyLevel.Insane => ErasureTable.EncInsane,
                RedundancyLevel.Paranoid => ErasureTable.EncParanoid,
                _ => throw new InvalidOperationException($"redundancy: level value {level} is not a legit redundancy level")
            };
        
        public static ErasureTable GetErasureTable(this RedundancyLevel level) =>
            level switch
            {
                RedundancyLevel.None => throw new InvalidOperationException("redundancy: level NONE does not have erasure table"),
                RedundancyLevel.Medium => ErasureTable.Medium,
                RedundancyLevel.Strong => ErasureTable.Strong,
                RedundancyLevel.Insane => ErasureTable.Insane,
                RedundancyLevel.Paranoid => ErasureTable.Paranoid,
                _ => throw new InvalidOperationException($"redundancy: level value {level} is not a legit redundancy level")
            };

        public static int GetEncParities(this RedundancyLevel level, int shards)
        {
            var erasureTable = level.GetEncErasureTable();
            return erasureTable.GetOptimalParities(shards);
        }
        
        public static int GetMaxEncShards(this RedundancyLevel level)
        {
            var p = level.GetEncParities(SwarmBmt.EncryptedBranches);
            return (SwarmBmt.Branches - p) / 2;
        }
        
        public static int GetMaxShards(this RedundancyLevel level)
        {
            var p = level.GetParities(SwarmBmt.Branches);
            return SwarmBmt.Branches - p;
        }

        public static int GetParities(this RedundancyLevel level, int shards)
        {
            var erasureTable = level.GetErasureTable();
            return erasureTable.GetOptimalParities(shards);
        }
    }
}