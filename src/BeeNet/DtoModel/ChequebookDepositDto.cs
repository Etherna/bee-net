namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class ChequebookDepositDto
    {
        public ChequebookDepositDto(string transactionHash)
        {
            TransactionHash = transactionHash;
        }
        
        public string TransactionHash { get; set; } = default!;
    }
}
