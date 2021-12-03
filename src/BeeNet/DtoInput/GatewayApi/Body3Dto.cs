#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoInput.GatewayApi
{
    public class Body3Dto : BaseDto
    {
        public Body3Dto(string address, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Address = address;
        }

        public string Address { get; set; }
    }
}

#pragma warning restore CA2227
