using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TransactionsDto
    {

        public TransactionsDto(Clients.v1_4.DebugApi.Response33 response33)
        {
            if (response33 == null)
            {
                return;
            }

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
        
        public string TransactionHash { get; set; } = default!;
        
        public string To { get; set; } = default!;

        public int Nonce { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string GasPrice { get; set; } = default!;
        
        public int GasLimit { get; set; } = default!;
        
        public string Data { get; set; } = default!;
        
        public DateTimeOffset Created { get; set; } = default!;
        
        public string Description { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Value { get; set; } = default!;
    }
}
