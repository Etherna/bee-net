using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class TransactionsDto : BaseDto
    {
        public TransactionsDto(ICollection<PendingTransactionsDto> pendingTransactions, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            PendingTransactions = pendingTransactions;
        }

        public ICollection<PendingTransactionsDto>
#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

PendingTransactions
        { get; set; }
    }
}

#pragma warning restore CA2227