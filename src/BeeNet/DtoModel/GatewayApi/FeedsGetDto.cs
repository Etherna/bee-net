﻿#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class FeedsGetDto : BaseDto
    {
        public FeedsGetDto(
            string reference, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Reference = reference;
        }

        public string Reference { get; set; }
    }
}

#pragma warning restore CA2227
