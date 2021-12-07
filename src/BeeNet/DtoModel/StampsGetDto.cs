namespace Etherna.BeeNet.DtoModel
{
    public class StampsGetDto
    {
        public StampsGetDto(bool exists, int batchTtl, object batchId, int utilization, bool usable, string label, int depth, string amount, int bucketDepth, int blockNumber, bool immutableFlag)
        {
            Exists = exists;
            BatchTTL = batchTtl;
            BatchID = batchId;
            Utilization = utilization;
            Usable = usable;
            Label = label;
            Depth = depth;
            Amount = amount;
            BucketDepth = bucketDepth;
            BlockNumber = blockNumber;
            ImmutableFlag = immutableFlag;
        }

        public bool Exists { get; set; } = default!;

        /// <summary>The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.</summary>
        public int BatchTTL { get; set; } = default!;

        public object BatchID { get; set; } = default!;

        public int Utilization { get; set; } = default!;

        /// <summary>Indicate that the batch was discovered by the Bee node, but it awaits enough on-chain confirmations before declaring the batch as usable.</summary>
        public bool Usable { get; set; } = default!;

        public string Label { get; set; } = default!;

        public int Depth { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Amount { get; set; } = default!;

        public int BucketDepth { get; set; } = default!;

        public int BlockNumber { get; set; } = default!;

        public bool ImmutableFlag { get; set; } = default!;
    }

}
