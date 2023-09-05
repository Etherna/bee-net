using System;
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class PostageBatchShortDto
    {
        // Constructors.
        internal PostageBatchShortDto(Clients.DebugApi.V5_0_0.Batches batch)
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

        internal PostageBatchShortDto(Clients.GatewayApi.V5_0_0.Batches batch)
        {
            if (batch is null)
                throw new ArgumentNullException(nameof(batch));

            BatchID = batch.BatchID;
            BatchTTL = Convert.ToInt64(batch.BatchTTL, CultureInfo.InvariantCulture); //TODO CAST to Long or change return type in String
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
