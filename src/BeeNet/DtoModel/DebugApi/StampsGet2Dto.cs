#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class StampsGet2Dto : BaseDto
    {
        public StampsGet2Dto(
            bool exists, 
            int batchTTL, 
            object batchId, 
            int utilization, 
            bool usable, 
            string label, 
            int depth, 
            string amount, 
            int bucketDepth, 
            int blockNumber, 
            bool immutableFlag, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Exists = exists;
            BatchTTL = batchTTL;
            BatchId = batchId;
            Utilization = utilization;
            Usable = usable;
            Label = label;
            Depth = depth;
            Amount = amount;
            BucketDepth = bucketDepth;
            BlockNumber = blockNumber;
            ImmutableFlag = immutableFlag;
        }

        public bool Exists { get; set; }

        /// <summary>The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.</summary>
        public int BatchTTL { get; set; }

        public object BatchId { get; set; }

        public int Utilization { get; set; }

        /// <summary>Indicate that the batch was discovered by the Bee node, but it awaits enough on-chain confirmations before declaring the batch as usable.</summary>
        public bool Usable { get; set; }

        public string Label { get; set; }

        public int Depth { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Amount { get; set; }

        public int BucketDepth { get; set; }

        public int BlockNumber { get; set; }

        public bool ImmutableFlag { get; set; }
    }
}

#pragma warning restore CA2227