#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class Cheque2Dto : BaseDto
    {
        public Cheque2Dto(
            ICollection<LastchequesDto>? lastCheques, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            LastCheques = lastCheques;
        }

        public ICollection<LastchequesDto>? LastCheques { get; set; }
    }
}

#pragma warning restore CA2227