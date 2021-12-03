#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class PendingTransactionsDto : BaseDto
    {
        public PendingTransactionsDto(
            string transactionHash, 
            string to, 
            int nonce, 
            string gasPrice, 
            int gasLimit, 
            string data, 
            DateTimeOffset created, 
            string description, 
            string value, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
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

        public string TransactionHash { get; set; }

        public string To { get; set; }

        public int Nonce { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string GasPrice { get; set; }

        public int GasLimit { get; set; }

        public string Data { get; set; }

        public System.DateTimeOffset Created { get; set; }

        public string Description { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Value { get; set; }
    }
}

#pragma warning restore CA2227