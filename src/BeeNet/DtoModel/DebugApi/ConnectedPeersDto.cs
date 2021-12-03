#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System.Collections.Generic;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ConnectedPeersDto : BaseDto
    {
        public ConnectedPeersDto(
            string address, 
            MetricsDto metrics,
            IDictionary<string, object> additionalProperties)
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