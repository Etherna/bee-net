using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TransactionsDto
    {
        // Constructors.
        public TransactionsDto(Clients.v1_4_1.DebugApi.Response33 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
            To = response.To;
            Nonce = response.Nonce;
            GasPrice = response.GasPrice;
            GasLimit = response.GasLimit;
            Data = response.Data;
            Created = response.Created;
            Description = response.Description;
            Value = response.Value;
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
