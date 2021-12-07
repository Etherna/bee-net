namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookWithdrawDto
    {
        public ChequebookWithdrawDto(string transactionHash)
        {
            TransactionHash = transactionHash;
        }
        
        public string TransactionHash { get; set; } = default!;
    }
}
