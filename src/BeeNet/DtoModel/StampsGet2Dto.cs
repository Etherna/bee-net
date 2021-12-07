namespace Etherna.BeeNet.DtoModel
{
    public class StampsGet2Dto
    {
        public StampsGet2Dto(bool exists, int batchTtl)
        {
            Exists = exists;
            BatchTTL = batchTtl;
        }

        public bool Exists { get; set; } = default!;

        /// <summary>The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.</summary>
        public int BatchTTL { get; set; } = default!;
    }
}
