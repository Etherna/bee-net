#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class Cheque2Dto : BaseDto
    {
        public Cheque2Dto(ICollection<LastchequesDto> lastcheques, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Lastcheques = lastcheques;
        }

        public ICollection<LastchequesDto> Lastcheques { get; set; }
    }
}

#pragma warning restore CA2227