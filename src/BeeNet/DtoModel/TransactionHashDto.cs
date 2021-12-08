namespace Etherna.BeeNet.DtoModel
{
    public class TransactionHashDto
    {
        public TransactionHashDto(string transactionHash)
        {
            TransactionHash = transactionHash;
        }

        public string TransactionHash { get; set; } = default!;
    }
}
