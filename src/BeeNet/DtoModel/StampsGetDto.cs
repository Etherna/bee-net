using System;

namespace Etherna.BeeNet.DtoModel
{
    public class StampsGetDto
    {
        // Constructors.
        public StampsGetDto(Clients.v1_4.DebugApi.Stamps stamps)
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

        public StampsGetDto(Clients.v1_4.DebugApi.Response37 response37)
        {
            if (response37 is null)
                throw new ArgumentNullException(nameof(response37));

            Exists = response37.Exists;
            BatchTTL = response37.BatchTTL;
            BatchID = response37.BatchID;
            Utilization = response37.Utilization;
            Usable = response37.Usable;
            Label = response37.Label;
            Depth = response37.Depth;
            Amount = response37.Amount;
            BucketDepth = response37.BucketDepth;
            BlockNumber = response37.BlockNumber;
            ImmutableFlag = response37.ImmutableFlag;
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
