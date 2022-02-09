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
