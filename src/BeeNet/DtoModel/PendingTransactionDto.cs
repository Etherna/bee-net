using System;

namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class PendingTransactionDto
    {
        public PendingTransactionDto(
            string transactionHash, 
            string to, 
            int nonce, 
            string gasPrice, 
            int gasLimit, 
            string data, 
            DateTimeOffset created, 
            string description, 
            string value)
        {
            TransactionHash = transactionHash;
            To = to;
            Nonce = nonce;
            GasPrice = gasPrice;
            GasLimit = gasLimit;
            Data = data;
            Created = created;
            Description = description;
            Value = value;
        }
        
        public string TransactionHash { get; set; } = default!;
        
        public string To { get; set; } = default!;

        public int Nonce { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string GasPrice { get; set; } = default!;

        public int GasLimit { get; set; } = default!;

        public string Data { get; set; } = default!;

        public System.DateTimeOffset Created { get; set; } = default!;

        public string Description { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Value { get; set; } = default!;
    }
}
