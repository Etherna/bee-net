using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequeBookBalanceDto
    {
        // Constructors.
        public ChequeBookBalanceDto(Clients.v1_4.DebugApi.Response8 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TotalBalance = response.TotalBalance;
            AvailableBalance = response.AvailableBalance;
        }


        // Properties.
        public string TotalBalance { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string AvailableBalance { get; }
    }
}
