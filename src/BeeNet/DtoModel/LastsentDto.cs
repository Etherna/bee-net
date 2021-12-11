namespace Etherna.BeeNet.DtoModel
{
    public class LastsentDto
    {
        public LastsentDto(Clients.v1_4.DebugApi.Lastsent lastsent)
        {
            if (lastsent is null)
            {
                return;
            }

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = lastsent.Payout;
        }

        public LastsentDto(Clients.v1_4.DebugApi.Lastsent2 lastsent2)
        {
            if (lastsent2 is null)
            {
                return;
            }

            Beneficiary = lastsent2.Beneficiary;
            Chequebook = lastsent2.Chequebook;
            Payout = lastsent2.Payout;
        }

        public string Beneficiary { get; set; } = default!;
        
        public string Chequebook { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; } = default!;
    }
}
