#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class Anonymous3Dto : BaseDto
    {
        public Anonymous3Dto(object batchID, int utilization, bool usable, string label, int depth, string amount, int bucketDepth, int blockNumber, bool immutableFlag, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            BatchID = batchID;
            Utilization = utilization;
            Usable = usable;
            Label = label;
            Depth = depth;
            Amount = amount;
            BucketDepth = bucketDepth;
            BlockNumber = blockNumber;
            ImmutableFlag = immutableFlag;
        }

        public object BatchID { get; set; }

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