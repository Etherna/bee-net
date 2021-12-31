using System;

namespace Etherna.BeeNet.DtoModel
{
    public class LastsentDto
    {
        // Constructors.
        public LastsentDto(Clients.v1_4_1.DebugApi.Lastsent lastsent)
        {
            if (lastsent is null)
                throw new ArgumentNullException(nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = lastsent.Payout;
        }

        public LastsentDto(Clients.v1_4_1.DebugApi.Lastsent2 lastsent)
        {
            if (lastsent is null)
                throw new ArgumentNullException(nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = lastsent.Payout;
        }


        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; }
    }
}
