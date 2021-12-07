namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookCashoutPostDto
    {
        public ChequebookCashoutPostDto(string transactionHash)
        {
            TransactionHash = transactionHash;
        }
        
        public string TransactionHash { get; set; } = default!;
    }
}
