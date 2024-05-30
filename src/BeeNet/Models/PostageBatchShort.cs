//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public sealed class PostageBatchShort
    {
        // Constructors.
        internal PostageBatchShort(Clients.Batches batch)
        {
            ArgumentNullException.ThrowIfNull(batch, nameof(batch));

            BatchId = batch.BatchID;
            BatchTtl = batch.BatchTTL;
            BucketDepth = batch.BucketDepth;
            Depth = batch.Depth;
            ImmutableFlag = batch.ImmutableFlag;
            Owner = batch.Owner;
            StartBlockNumber = batch.Start;
            StorageRadius = batch.StorageRadius;
            Value = batch.Value;
        }

        // Properties.
        public PostageBatchId BatchId { get; }
        public long BatchTtl { get; }
        public int BucketDepth { get; }
        public int Depth { get; }
        public bool ImmutableFlag { get; }
        public string Owner { get; }
        public int StartBlockNumber { get; }
        public int StorageRadius { get; }
        public string Value { get; }
    }
}
