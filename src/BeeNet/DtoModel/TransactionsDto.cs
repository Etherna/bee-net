namespace Etherna.BeeNet.DtoModel
{
    public class TransactionsDto
    {
        public TransactionsDto(string transactionHash)
        {
            TransactionHash = transactionHash;
        }

        public string TransactionHash { get; set; } = default!;
    }
}
