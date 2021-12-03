#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class LastCashedChequeDto : BaseDto
    {
        public LastCashedChequeDto(string beneficiary, string chequebook, string payout, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Beneficiary = beneficiary;
            Chequebook = chequebook;
            Payout = payout;
        }

        public string Beneficiary { get; set; }

        public string Chequebook { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; }
    }
}

#pragma warning restore CA2227