#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class RefreshDto : BaseDto
    {
        public RefreshDto(
            string key, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}

#pragma warning restore CA2227
