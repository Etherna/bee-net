using System;

namespace Etherna.BeeNet.DtoModel
{
    public class BatchDto
    {
        // Constructors.
        public BatchDto(Clients.v1_4_1.DebugApi.Response39 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BatchId = response.BatchID;
        }

        public BatchDto(Clients.v1_4_1.DebugApi.Response40 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BatchId = response.BatchID;
        }

        public BatchDto(Clients.v1_4_1.DebugApi.Response41 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BatchId = response.BatchID;
        }


        // Properties.
        public object BatchId { get; }
    }
}
