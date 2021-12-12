using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookBalanceDto
    {
        // Constructors.
        public ChequebookBalanceDto(Clients.v1_4.DebugApi.Response8 response8)
        {
            if (response8 is null)
                throw new ArgumentNullException(nameof(response8));

            TotalBalance = response8.TotalBalance;
            AvailableBalance = response8.AvailableBalance;
        }


        // Properties.
        public string TotalBalance { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string AvailableBalance { get; }
    }
}
