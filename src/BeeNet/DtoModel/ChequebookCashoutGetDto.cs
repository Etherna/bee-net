using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequeBookCashoutGetDto
    {
        // Constructors.
        public ChequeBookCashoutGetDto(Clients.v1_4_1.DebugApi.Response25 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Peer = response.Peer;
            LastCashedCheque = new LastCashedChequeDto(response.LastCashedCheque);
            TransactionHash = response.TransactionHash;
            Result = new ResultChequeBookDto(response.Result);
            UncashedAmount = response.UncashedAmount;
        }


        // Properties.
        public string Peer { get; }
        public LastCashedChequeDto LastCashedCheque { get; }
        public string TransactionHash { get; }
        public ResultChequeBookDto Result { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string UncashedAmount { get; }
    }

}
