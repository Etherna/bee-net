using System;

namespace Etherna.BeeNet.DtoModel
{
    public class LastCashedChequeDto
    {
        // Constructors.
        public LastCashedChequeDto(Clients.v1_4_1.DebugApi.LastCashedCheque lastCashedCheque)
        {
            if (lastCashedCheque is null)
                throw new ArgumentNullException(nameof(lastCashedCheque));

            Beneficiary = lastCashedCheque.Beneficiary;
            Chequebook = lastCashedCheque.Chequebook;
            Payout = lastCashedCheque.Payout;
        }


        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; }
    }
}
