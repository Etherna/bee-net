#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class PingpongDto : BaseDto
    {
        public PingpongDto(
            string rtt, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Rtt = rtt;
        }

        public string Rtt { get; set; }
    }
}

#pragma warning restore CA2227