namespace Etherna.BeeNet.DtoModel
{
    public class LastchequesDto
    {
        public LastchequesDto(
            string peer, 
            Lastreceived2 lastreceived, 
            Lastsent2 lastsent)
        {
            Peer = peer;
            Lastreceived = lastreceived;
            Lastsent = lastsent;
        }
        
        public string Peer { get; set; } = default!;

        public Lastreceived2 Lastreceived { get; set; } = default!;

        public Lastsent2 Lastsent { get; set; } = default!;
    }

    public class Lastreceived2
    {
        public string Beneficiary { get; set; } = default!;
        
        public string Chequebook { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; } = default!;
        
    }

    public class Lastsent2
    {
        public string Beneficiary { get; set; } = default!;
        
        public string Chequebook { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; } = default!;

    }

}
