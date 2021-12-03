#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class Lastsent2Dto : BaseDto
    {
        public Lastsent2Dto(
            string beneficiary, 
            string chequeBook, 
            string payout, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Beneficiary = beneficiary;
            ChequeBook = chequeBook;
            Payout = payout;
        }

        public string Beneficiary { get; set; }

        public string ChequeBook { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; }
    }
}

#pragma warning restore CA2227