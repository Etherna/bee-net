using System;

namespace Etherna.BeeNet.DtoModels
{
    public class PostageBatchShortDto
    {
        public PostageBatchShortDto(Clients.DebugApi.V1_2_1.Stamps2 stamps)
        {
            if (stamps is null)
                throw new ArgumentNullException(nameof(stamps));

            BatchID = stamps.BatchID;
            BatchTTL = stamps.BatchTTL;
            BucketDepth = stamps.BucketDepth;
            Depth = stamps.Depth;
            ImmutableFlag = stamps.ImmutableFlag;
            Owner = stamps.Owner;
            StartBlockNumber = stamps.BlockNumber;
            Value = stamps.Value;
        }

        public PostageBatchShortDto(Clients.DebugApi.V2_0_0.Batches batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            BatchID = batch.BatchID;
            BatchTTL = batch.BatchTTL;
            BucketDepth = batch.BucketDepth;
            Depth = batch.Depth;
            ImmutableFlag = batch.ImmutableFlag;
            Owner = batch.Owner;
            StartBlockNumber = batch.BlockNumber;
            Value = batch.Value;
        }

        public PostageBatchShortDto(Clients.DebugApi.V2_0_1.Batches batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            BatchID = batch.BatchID;
            BatchTTL = batch.BatchTTL;
            BucketDepth = batch.BucketDepth;
            Depth = batch.Depth;
            ImmutableFlag = batch.ImmutableFlag;
            Owner = batch.Owner;
            StartBlockNumber = batch.Start;
            StorageRadius = batch.StorageRadius;
            Value = batch.Value;
        }

        // Properties.
        public string BatchID { get; }
        public long BatchTTL { get; }
        public int BucketDepth { get; }
        public int Depth { get; }
        public bool ImmutableFlag { get; }
        public string Owner { get; }
        public int StartBlockNumber { get; }
        public int StorageRadius { get; }
        public string Value { get; }
    }
}
