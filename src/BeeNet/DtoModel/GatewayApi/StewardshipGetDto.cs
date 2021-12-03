#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class StewardshipGetDto : BaseDto
    {
        public StewardshipGetDto(
            bool isRetrievable, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            IsRetrievable = isRetrievable;
        }

        public bool IsRetrievable { get; set; }
    }
}

#pragma warning restore CA2227
