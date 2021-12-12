using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TransactionsDto
    {
        // Constructors.
        public TransactionsDto(Clients.v1_4.DebugApi.Response33 response33)
        {
            if (response33 is null)
                throw new ArgumentNullException(nameof(response33));

            TransactionHash = response33.TransactionHash;
            To = response33.To;
            Nonce = response33.Nonce;
            GasPrice = response33.GasPrice;
            GasLimit = response33.GasLimit;
            Data = response33.Data;
            Created = response33.Created;
            Description = response33.Description;
            Value = response33.Value;
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
