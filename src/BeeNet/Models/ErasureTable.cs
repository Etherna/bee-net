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

using System;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Models
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    [SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
    public class ErasureTable
    {
        // Constructor.
        public ErasureTable(int[] shards, int[] parities)
        {
            ArgumentNullException.ThrowIfNull(shards, nameof(shards));
            ArgumentNullException.ThrowIfNull(parities, nameof(parities));

            if (shards.Length != parities.Length)
                throw new InvalidOperationException(
                    "redundancy table: shards and parities arrays must be of equal size");

            var maxShards = shards[0];
            var maxParities = parities[0];
            for (var k = 1; k < shards.Length; k++)
            {
                var s = shards[k];
                if (maxShards <= s)
                    throw new InvalidOperationException(
                        "redundancy table: shards should be in strictly descending order");
                var p = parities[k];
                if (maxParities <= p)
                    throw new InvalidOperationException(
                        "redundancy table: parities should be in strictly descending order");
                maxShards = s;
                maxParities = p;
            }

            Shards = shards;
            Parities = parities;
        }
            
        // Properties.
        public int[] Shards { get; }
        public int[] Parities { get; }
        
        // Static properties.
        public static ErasureTable Medium { get; } = new(
            new[] { 95, 69, 47, 29, 15, 6, 2, 1 },
            new[] { 9, 8, 7, 6, 5, 4, 3, 2 });

        public static ErasureTable EncMedium = new(
            new[] { 47, 34, 23, 14, 7, 3, 1 },
            new[] { 9, 8, 7, 6, 5, 4, 3 }
        );

        public static ErasureTable Strong = new(
            new[] { 105, 96, 87, 78, 70, 62, 54, 47, 40, 33, 27, 21, 16, 11, 7, 4, 2, 1 },
            new[] { 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4 }
        );

        public static ErasureTable EncStrong = new(
            new[] { 52, 48, 43, 39, 35, 31, 27, 23, 20, 16, 13, 10, 8, 5, 3, 2, 1 },
            new[] { 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5 }
        );

        public static ErasureTable Insane = new(
            new[]
            {
                93, 88, 83, 78, 74, 69, 64, 60, 55, 51, 46, 42, 38, 34, 30, 27, 23, 20, 17, 14, 11, 9, 6, 4, 3, 2, 1
            },
            new[]
            {
                31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5
            }
        );

        public static ErasureTable EncInsane = new(
            new[] { 46, 44, 41, 39, 37, 34, 32, 30, 27, 25, 23, 21, 19, 17, 15, 13, 11, 10, 8, 7, 5, 4, 3, 2, 1 },
            new[] { 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 6 }
        );

        public static ErasureTable Paranoid = new(
            new[]
            {
                37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18,
                17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1,
            },
            new[]
            {
                89, 87, 86, 84, 83, 81, 80, 78, 76, 75, 73, 71, 70, 68, 66, 65, 63, 61, 59, 58,
                56, 54, 52, 50, 48, 47, 45, 43, 40, 38, 36, 34, 31, 29, 26, 23, 19,
            }
        );

        public static ErasureTable EncParanoid = new(
            new[]
            {
                18, 17, 16, 15, 14, 13, 12, 11, 10, 9,
                8, 7, 6, 5, 4, 3, 2, 1,
            },
            new[]
            {
                87, 84, 81, 78, 75, 71, 68, 65, 61, 58,
                54, 50, 47, 43, 38, 34, 29, 23,
            }
        );
        
        // Methods.
        /// <summary>
        /// Gives back the optimal parity number for a given shard
        /// </summary>
        /// <param name="maxShards"></param>
        /// <returns></returns>
        public int GetOptimalParities(int maxShards)
        {
            for (int k = 0; k < Shards.Length; k++)
            {
                if (maxShards >= Shards[k])
                    return Parities[k];
            }

            return 0;
        }
    }
}