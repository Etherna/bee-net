namespace Etherna.BeeNet.DtoModel
{
    public class BatchDto
    {
        public BatchDto(object batchId)
        {
            BatchId = batchId;
        }

        public object BatchId { get; set; } = default!;
    }
}
