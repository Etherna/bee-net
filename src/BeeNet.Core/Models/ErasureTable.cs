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
    public class ErasureTable
    {
        // Fields.
        private readonly int[] _shards;
        private readonly int[] _parities;

        // Constructor.
        private ErasureTable(int[] shards, int[] parities)
        {
            ArgumentNullException.ThrowIfNull(shards, nameof(shards));
            ArgumentNullException.ThrowIfNull(parities, nameof(parities));

            if (shards.Length != parities.Length)
                throw new InvalidOperationException("Shards and parities arrays must be of equal size");

            var maxShard = shards[0];
            var maxParity = parities[0];
            for (var i = 1; i < shards.Length; i++)
            {
                var shard = shards[i];
                if (maxShard <= shard)
                    throw new InvalidOperationException("Shards should be in strictly descending order");
                var parity = parities[i];
                if (maxParity <= parity)
                    throw new InvalidOperationException("Parities should be in strictly descending order");
                maxShard = shard;
                maxParity = parity;
            }

            _shards = shards;
            _parities = parities;
        }
            
        // Properties.
        public IReadOnlyCollection<int> Shards => _shards;
        public IReadOnlyCollection<int> Parities => _parities;
        
        // Static properties.
        public static ErasureTable Medium { get; } = new(
            [95, 69, 47, 29, 15, 6, 2, 1],
            [9 , 8 , 7 , 6 , 5 , 4, 3, 2]);

        public static ErasureTable EncMedium { get; } = new(
            [47, 34, 23, 14, 7, 3, 1],
            [9 , 8 , 7 , 6 , 5, 4, 3]);

        public static ErasureTable Strong { get; } = new(
            [105, 96, 87, 78, 70, 62, 54, 47, 40, 33, 27, 21, 16, 11, 7, 4, 2, 1],
            [21 , 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9 , 8 , 7, 6, 5, 4]);

        public static ErasureTable EncStrong { get; } = new(
            [52, 48, 43, 39, 35, 31, 27, 23, 20, 16, 13, 10, 8, 5, 3, 2, 1],
            [21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5]);

        public static ErasureTable Insane { get; } = new(
            [93, 88, 83, 78, 74, 69, 64, 60, 55, 51, 46, 42, 38, 34, 30, 27, 23, 20, 17, 14, 11, 9 , 6, 4, 3, 2, 1],
            [31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5]);

        public static ErasureTable EncInsane { get; } = new(
            [46, 44, 41, 39, 37, 34, 32, 30, 27, 25, 23, 21, 19, 17, 15, 13, 11, 10, 8 , 7 , 5 , 4 , 3, 2, 1],
            [31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 6]);

        public static ErasureTable Paranoid { get; } = new(
            [37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9 , 8 , 7 , 6 , 5 , 4 , 3 , 2 , 1],
            [89, 87, 86, 84, 83, 81, 80, 78, 76, 75, 73, 71, 70, 68, 66, 65, 63, 61, 59, 58, 56, 54, 52, 50, 48, 47, 45, 43, 40, 38, 36, 34, 31, 29, 26, 23, 19]);

        public static ErasureTable EncParanoid { get; } = new(
            [18, 17, 16, 15, 14, 13, 12, 11, 10, 9 , 8 , 7 , 6 , 5 , 4 , 3 , 2 , 1],
            [87, 84, 81, 78, 75, 71, 68, 65, 61, 58, 54, 50, 47, 43, 38, 34, 29, 23]);
        
        // Methods.
        /// <summary>
        /// Gives back the optimal parity number for a given shard
        /// </summary>
        public int GetOptimalParities(int maxShards)
        {
            for (var i = 0; i < _shards.Length; i++)
                if (maxShards >= _shards[i])
                    return _parities[i];

            return 0;
        }
    }
}