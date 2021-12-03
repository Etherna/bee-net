using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"
namespace Etherna.BeeNet.DtoModel.Debug
{
    public class TransactionsDto : BaseDto
    {
        public TransactionsDto(ICollection<PendingTransactionsDto>? pendingTransactions, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            PendingTransactions = pendingTransactions;
        }

        public ICollection<PendingTransactionsDto>? PendingTransactions { get; set; }
        
    }
}
#pragma warning restore CA2227