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

using Epoche;
using System;
using System.Collections.Concurrent;

namespace Etherna.BeeNet.Services.Pipelines.Models
{
    public class BmtPool //evaluate to remove
    {
        // Consts.
        private const int BmtBranches = 128;
        private const int Capacity = 32;
        
        // Fields.
        private readonly BmtPoolConfig config;
        private readonly ConcurrentQueue<BmtTree> trees = new();

        // Constructor.
        private BmtPool(BmtPoolConfig config)
        {
            this.config = config;
            for (int i = 0; i < config.Capacity; i++)
                trees.Enqueue(new BmtTree(config.SegmentSize, config.MaxSize, config.Depth, config.Hasher));
        }

        // Static properties.
        public static BmtPool Instance { get; } = new BmtPool(
            new BmtPoolConfig(
                Keccak256.ComputeHash,
                BmtBranches,
                Capacity));

        // Methods.
        public void Put(BmtHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            trees.Enqueue(hasher.Bmt);
        }

        public bool TryGet(out BmtHasher? hasher)
        {
            if (!trees.TryDequeue(out var tree))
            {
                hasher = null;
                return false;
            }

            hasher = new BmtHasher(
                config,
                result: make(chan[]byte),
                errc: make(chan error, 1),
                span: make([]byte, SpanSize),
                bmt: tree);
            return true;
        }
    }
}