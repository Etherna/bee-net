#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class PinsGet2Dto : BaseDto
    {
        public PinsGet2Dto(ICollection<string> addresses, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Addresses = addresses;
        }

        public ICollection<string> Addresses { get; set; }
    }
}

#pragma warning restore CA2227
