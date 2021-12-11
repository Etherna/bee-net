using System;

namespace Etherna.BeeNet.DtoModel
{
    public class PendingTransactionDto
    {
        public PendingTransactionDto(Clients.v1_4.DebugApi.PendingTransactions pendingTransactions)
        {
            if (pendingTransactions is null)
            {
                return;
            }

            TransactionHash = pendingTransactions.TransactionHash;
            To = pendingTransactions.To;
            Nonce = pendingTransactions.Nonce;
            GasPrice = pendingTransactions.GasPrice;
            GasLimit = pendingTransactions.GasLimit;
            Data = pendingTransactions.Data;
            Created = pendingTransactions.Created;
            Description = pendingTransactions.Description;
            Value = pendingTransactions.Value;
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
