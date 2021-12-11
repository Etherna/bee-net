namespace Etherna.BeeNet.DtoModel
{
    public class BatchDto
    {
        public BatchDto(Clients.v1_4.DebugApi.Response39 response39)
        {
            if (response39 is null)
            {
                return;
            }

            BatchId = response39.BatchID;
        }

        public BatchDto(Clients.v1_4.DebugApi.Response40 response40)
        {
            if (response40 is null)
            {
                return;
            }

            BatchId = response40.BatchID;
        }

        public BatchDto(Clients.v1_4.DebugApi.Response41 response41)
        {
            if (response41 is null)
            {
                return;
            }

            BatchId = response41.BatchID;
        }

        public object BatchId { get; set; } = default!;
    }
}
