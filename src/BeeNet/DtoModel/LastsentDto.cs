namespace Etherna.BeeNet.DtoModel
{
    public class LastsentDto
    {
        public LastsentDto(
            string beneficiary, 
            string chequebook, 
            string payout)
        {
            Beneficiary = beneficiary;
            Chequebook = chequebook;
            Payout = payout;
        }

        public string Beneficiary { get; set; } = default!;
        
        public string Chequebook { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; set; } = default!;
    }
}
