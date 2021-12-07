namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookCashoutGetResponse
    {
        public ChequebookCashoutGetResponse(
            string peer, 
            LastCashedCheque lastCashedCheque, 
            string transactionHash, 
            Result result, 
            string uncashedAmount)
        {
            Peer = peer;
            LastCashedCheque = lastCashedCheque;
            TransactionHash = transactionHash;
            Result = result;
            UncashedAmount = uncashedAmount;
        }
        
        public string Peer { get; set; } = default!;
        
        public LastCashedCheque LastCashedCheque { get; set; } = default!;
        
        public string TransactionHash { get; set; } = default!;
        
        public Result Result { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string UncashedAmount { get; set; } = default!;
    }

    public class LastCashedCheque
    {
        public LastCashedCheque(string beneficiary, string chequebook, string payout)
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

    public class Result
    {
        public string Recipient { get; set; } = default!;

        public Result(string recipient, string lastPayout, bool bounced)
        {
            Recipient = recipient;
            LastPayout = lastPayout;
            Bounced = bounced;
        }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string LastPayout { get; set; } = default!;

        public bool Bounced { get; set; } = default!;
    }
}
