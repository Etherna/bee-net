using System;

namespace Etherna.BeeNet.DtoModel
{
    public class BatchDto
    {
        // Constructors.
        public BatchDto(Clients.v1_4.DebugApi.Response39 response39)
        {
            if (response39 is null)
                throw new ArgumentNullException(nameof(response39));

            BatchId = response39.BatchID;
        }

        public BatchDto(Clients.v1_4.DebugApi.Response40 response40)
        {
            if (response40 is null)
                throw new ArgumentNullException(nameof(response40));

            BatchId = response40.BatchID;
        }

        public BatchDto(Clients.v1_4.DebugApi.Response41 response41)
        {
            if (response41 is null)
                throw new ArgumentNullException(nameof(response41));

            BatchId = response41.BatchID;
        }


        // Properties.
        public object BatchId { get; }
    }
}
