#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class DisconnectedPeersDto : BaseDto
    {
        public DisconnectedPeersDto(string address, MetricsDto metrics, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Address = address;
            Metrics = metrics;
        }

        public string Address { get; set; }

        public MetricsDto Metrics { get; set; }
    }
}

#pragma warning restore CA2227