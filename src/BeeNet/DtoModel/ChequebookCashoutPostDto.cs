namespace Etherna.BeeNet.DtoModel.DebugApi
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
