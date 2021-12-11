namespace Etherna.BeeNet.DtoModel
{
    public class LastreceivedDto
    {
        public LastreceivedDto(Clients.v1_4.DebugApi.Lastreceived lastreceived)
        {
            if (lastreceived is null)
            {
                return;
            }

            Beneficiary = lastreceived.Beneficiary;
            Chequebook = lastreceived.Chequebook;
            Payout = lastreceived.Payout;
        }

        public LastreceivedDto(Clients.v1_4.DebugApi.Lastreceived2 lastreceived2)
        {
            if (lastreceived2 is null)
            {
                return;
            }

            Beneficiary = lastreceived2.Beneficiary;
            Chequebook = lastreceived2.Chequebook;
            Payout = lastreceived2.Payout;
        }

        public string Beneficiary { get; set; } = default!;
        
        public string Chequebook { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; } = default!;
    }
}
