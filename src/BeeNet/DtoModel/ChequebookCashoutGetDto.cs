using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookCashoutGetDto
    {
        // Constructors.
        public ChequebookCashoutGetDto(Clients.v1_4.DebugApi.Response25 response25)
        {
            if (response25 is null)
                throw new ArgumentNullException(nameof(response25));

            Peer = response25.Peer;
            LastCashedCheque = new LastCashedChequeDto(response25.LastCashedCheque);
            TransactionHash = response25.TransactionHash;
            Result = new ResultChequebookDto(response25.Result);
            UncashedAmount = response25.UncashedAmount;
        }


        // Properties.
        public string Peer { get; }
        public LastCashedChequeDto LastCashedCheque { get; }
        public string TransactionHash { get; }
        public ResultChequebookDto Result { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string UncashedAmount { get; }
    }

}
