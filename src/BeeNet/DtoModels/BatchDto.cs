//   Copyright 2021-present Etherna Sagl
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

namespace Etherna.BeeNet.DtoModels
{
    public class BatchDto
    {
        public BatchDto(Clients.DebugApi.V1_2_1.Stamps2 stamps)
        {
            if (stamps is null)
                throw new ArgumentNullException(nameof(stamps));

            BatchID = stamps.BatchID;
            BatchTTL = stamps.BatchTTL;
            BlockNumber = stamps.BlockNumber;
            BucketDepth = stamps.BucketDepth;
            Depth = stamps.Depth;
            ImmutableFlag = stamps.ImmutableFlag;
            Value = stamps.Value;
            Owner = stamps.Owner;
        }

        public BatchDto(Clients.DebugApi.V2_0_0.Batches batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            BatchID = batch.BatchID;
            BatchTTL = batch.BatchTTL;
            BlockNumber = batch.BlockNumber;
            BucketDepth = batch.BucketDepth;
            Depth = batch.Depth;
            ImmutableFlag = batch.ImmutableFlag;
            Value = batch.Value;
            Owner = batch.Owner;
        }

        // Properties.
        public string BatchID { get; set; } = default!;
        public int BatchTTL { get; set; } = default!;
        public int BlockNumber { get; set; } = default!;
        public int BucketDepth { get; set; } = default!;
        public int Depth { get; set; } = default!;
        public bool ImmutableFlag { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string Owner { get; set; } = default!;
    }
}
