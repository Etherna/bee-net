namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookBalanceDto
    {
        public ChequebookBalanceDto(Clients.v1_4.DebugApi.Response8 response8)
        {
            if (response8 is null)
            {
                return;
            }

            TotalBalance = response8.TotalBalance;
            AvailableBalance = response8.AvailableBalance;
        }

        public string TotalBalance { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string AvailableBalance { get; set; } = default!;
    }
}
