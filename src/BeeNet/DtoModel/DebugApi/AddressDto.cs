#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class AddressDto : BaseDto
    {
        public AddressDto(
            string chequeBookAddress, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            ChequeBookAddress = chequeBookAddress;
        }

        public string ChequeBookAddress { get; set; }
    }
}

#pragma warning restore CA2227