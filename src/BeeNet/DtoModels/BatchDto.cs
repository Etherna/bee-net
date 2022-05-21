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

        public BatchDto(Clients.DebugApi.V2_0_1.Batches batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            BatchID = batch.BatchID;
            BatchTTL = batch.BatchTTL;
            //BlockNumber = batch.BlockNumber; //TODO review: Missing BlockNumber
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
