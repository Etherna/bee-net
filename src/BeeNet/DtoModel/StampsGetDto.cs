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

namespace Etherna.BeeNet.DtoModel
{
    public class StampsGetDto
    {
        // Constructors.
        public StampsGetDto(Clients.DebugApi.v1_2_0.Stamps stamps)
        {
            if (stamps is null)
                throw new ArgumentNullException(nameof(stamps));

            Exists = stamps.Exists;
            BatchTTL = stamps.BatchTTL;
            BatchID = stamps.BatchID;
            Utilization = stamps.Utilization;
            Usable = stamps.Usable;
            Label = stamps.Label;
            Depth = stamps.Depth;
            Amount = stamps.Amount;
            BucketDepth = stamps.BucketDepth;
            BlockNumber = stamps.BlockNumber;
            ImmutableFlag = stamps.ImmutableFlag;
        }

        public StampsGetDto(Clients.DebugApi.v1_2_0.Response37 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Exists = response.Exists;
            BatchTTL = response.BatchTTL;
            BatchID = response.BatchID;
            Utilization = response.Utilization;
            Usable = response.Usable;
            Label = response.Label;
            Depth = response.Depth;
            Amount = response.Amount;
            BucketDepth = response.BucketDepth;
            BlockNumber = response.BlockNumber;
            ImmutableFlag = response.ImmutableFlag;
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
