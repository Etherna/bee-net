#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class AddressDto : BaseDto
    {
        public AddressDto(string chequebookAddress, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            ChequebookAddress = chequebookAddress;
        }

        public string ChequebookAddress { get; set; }
    }
}

#pragma warning restore CA2227