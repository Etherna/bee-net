#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoInput.GatewayApi
{
    public class BodyDto : BaseDto
    {
        public BodyDto(string role, 
            int expiry, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Role = role;
            Expiry = expiry;
        }

        public string Role { get; set; }

        public int Expiry { get; set; }
    }
}

#pragma warning restore CA2227
