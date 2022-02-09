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
    public class StampsGetDto
    {
        // Constructors.
        public StampsGetDto(Clients.DebugApi.V1_2_0.Stamps batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            Exists = batch.Exists;
            BatchTTL = batch.BatchTTL;
            BatchID = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            Amount = batch.Amount;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        public StampsGetDto(Clients.DebugApi.V1_2_0.Response37 batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            Exists = batch.Exists;
            BatchTTL = batch.BatchTTL;
            BatchID = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            Amount = batch.Amount;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        public StampsGetDto(Clients.DebugApi.V1_2_1.Stamps batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            Exists = batch.Exists;
            BatchTTL = batch.BatchTTL;
            BatchID = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            Amount = batch.Amount;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        public StampsGetDto(Clients.DebugApi.V1_2_1.Response38 batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            Exists = batch.Exists;
            BatchTTL = batch.BatchTTL;
            BatchID = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            Amount = batch.Amount;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        // Properties.
        public bool Exists { get; }
        /// <summary>The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.</summary>
        public int BatchTTL { get; }
        public object BatchID { get; }
        public int Utilization { get; }
        /// <summary>Indicate that the batch was discovered by the Bee node, but it awaits enough on-chain confirmations before declaring the batch as usable.</summary>
        public bool Usable { get; }
        public string Label { get; }
        public int Depth { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Amount { get; }
        public int BucketDepth { get; }
        public int BlockNumber { get; }
        public bool ImmutableFlag { get; }
    }

}
