#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ConsumedDto: BaseDto
    {
        public ConsumedDto(
            ICollection<Balances2Dto>? balances, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Balances = balances;
        }

        public ICollection<Balances2Dto>? Balances { get; set; }
    }
}

#pragma warning restore CA2227