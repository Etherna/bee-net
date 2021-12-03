#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class TransactionsDeleteDto : BaseDto
    {
        public TransactionsDeleteDto(string transactionHash, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            TransactionHash = transactionHash;
        }

        public string TransactionHash { get; set; }
    }
}

#pragma warning restore CA2227