#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel
{
    public class BaseDto
    {
        public BaseDto(IDictionary<string, object> additionalProperties)
        {
            AdditionalProperties = additionalProperties;
        }

        public IDictionary<string, object> AdditionalProperties { get; set; }
    }
}

#pragma warning restore CA2227
