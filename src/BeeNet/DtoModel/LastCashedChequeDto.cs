using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel
{
    public class LastCashedChequeDto
    {
        public LastCashedChequeDto(Clients.v1_4.DebugApi.LastCashedCheque lastCashedCheque)
        {
            if (lastCashedCheque is null)
            {
                return;
            }

            Beneficiary = lastCashedCheque.Beneficiary;
            Chequebook = lastCashedCheque.Chequebook;
            Payout = lastCashedCheque.Payout;
        }

        public string Beneficiary { get; set; } = default!;

        public string Chequebook { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; } = default!;
    }
}
