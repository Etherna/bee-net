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

using Etherna.BeeNet.Clients.DebugApi.V1_2_1;
using System;

namespace Etherna.BeeNet.DtoModels
{
    public class ValidPostageBatchDto
    {
        public ValidPostageBatchDto(Stamps2 validBatch)
        {
            if (validBatch is null)
                throw new ArgumentNullException(nameof(validBatch));

            BatchID = validBatch.BatchID;
            BatchTTL = validBatch.BatchTTL;
            BlockNumber = validBatch.BlockNumber;
            BucketDepth = validBatch.BucketDepth;
            Depth = validBatch.Depth;
            ImmutableFlag = validBatch.ImmutableFlag;
            Owner = validBatch.Owner;
            Value = validBatch.Value;
        }

        public string BatchID { get; }
        public int BatchTTL { get; }
        public int BlockNumber { get; }
        public int BucketDepth { get; }
        public int Depth { get; }
        /// <summary>
        /// Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)
        /// </summary>
        public bool ImmutableFlag { get; }
        public string Owner { get; }
        public string Value { get; }
    }
}
