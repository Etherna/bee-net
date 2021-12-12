using System;

namespace Etherna.BeeNet.DtoModel
{
    public class LastreceivedDto
    {
        // Constructors.
        public LastreceivedDto(Clients.v1_4.DebugApi.Lastreceived lastreceived)
        {
            if (lastreceived is null)
                throw new ArgumentNullException(nameof(lastreceived));

            Beneficiary = lastreceived.Beneficiary;
            Chequebook = lastreceived.Chequebook;
            Payout = lastreceived.Payout;
        }

        public LastreceivedDto(Clients.v1_4.DebugApi.Lastreceived2 lastreceived2)
        {
            if (lastreceived2 is null)
                throw new ArgumentNullException(nameof(lastreceived2));

            Beneficiary = lastreceived2.Beneficiary;
            Chequebook = lastreceived2.Chequebook;
            Payout = lastreceived2.Payout;
        }


        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; }
    }
}
