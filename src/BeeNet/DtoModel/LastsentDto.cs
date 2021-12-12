using System;

namespace Etherna.BeeNet.DtoModel
{
    public class LastsentDto
    {
        // Constructors.
        public LastsentDto(Clients.v1_4.DebugApi.Lastsent lastsent)
        {
            if (lastsent is null)
                throw new ArgumentNullException(nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = lastsent.Payout;
        }

        public LastsentDto(Clients.v1_4.DebugApi.Lastsent2 lastsent2)
        {
            if (lastsent2 is null)
                throw new ArgumentNullException(nameof(lastsent2));

            Beneficiary = lastsent2.Beneficiary;
            Chequebook = lastsent2.Chequebook;
            Payout = lastsent2.Payout;
        }


        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; }
    }
}
