namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class ChequebookBalanceDto
    {
        public ChequebookBalanceDto(
            string totalBalance, 
            string availableBalance)
        {
            TotalBalance = totalBalance;
            AvailableBalance = availableBalance;
        }

        public string TotalBalance { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string AvailableBalance { get; set; } = default!;
    }
}
