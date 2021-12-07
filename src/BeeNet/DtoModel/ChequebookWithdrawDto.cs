namespace Etherna.BeeNet.DtoModel.DebugApi
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
