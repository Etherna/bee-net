using System;

namespace Etherna.BeeNet.DtoModel
{
    public class LastreceivedDto
    {
        // Constructors.
        public LastreceivedDto(Clients.v1_4_1.DebugApi.Lastreceived lastReceived)
        {
            if (lastReceived is null)
                throw new ArgumentNullException(nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = lastReceived.Payout;
        }

        public LastreceivedDto(Clients.v1_4_1.DebugApi.Lastreceived2 lastReceived)
        {
            if (lastReceived is null)
                throw new ArgumentNullException(nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = lastReceived.Payout;
        }


        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; }
    }
}
