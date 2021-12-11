namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookCashoutGetDto
    {
        public ChequebookCashoutGetDto(Clients.v1_4.DebugApi.Response25 response25)
        {
            if (response25 is null)
            {
                return;
            }

            Peer = response25.Peer;
            LastCashedCheque = new LastCashedChequeDto(response25.LastCashedCheque);
            TransactionHash = response25.TransactionHash;
            Result = new ResultChequebookDto(response25.Result);
            UncashedAmount = response25.UncashedAmount;
        }
        
        public string Peer { get; set; } = default!;
        
        public LastCashedChequeDto LastCashedCheque { get; set; } = default!;
        
        public string TransactionHash { get; set; } = default!;
        
        public ResultChequebookDto Result { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string UncashedAmount { get; set; } = default!;
    }

}
