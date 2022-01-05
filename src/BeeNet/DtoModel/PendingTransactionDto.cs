using System;

namespace Etherna.BeeNet.DtoModel
{
    public class PendingTransactionDto
    {
        // Constructors.
        public PendingTransactionDto(Clients.v1_4_1.DebugApi.PendingTransactions pendingTransactions)
        {
            if (pendingTransactions is null)
                throw new ArgumentNullException(nameof(pendingTransactions));

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


        // Properties.
        public string TransactionHash { get; }
        public string To { get; }
        public int Nonce { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string GasPrice { get; }
        public int GasLimit { get; }
        public string Data { get; }
        public DateTimeOffset Created { get; }
        public string Description { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Value { get; }
    }
}
