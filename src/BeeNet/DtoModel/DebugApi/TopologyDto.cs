#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class TopologyDto : BaseDto
    {
        public TopologyDto(string baseAddr, int population, int connected, string timestamp, int nnLowWatermark, int depth, IDictionary<string, AnonymousDto> bins, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            BaseAddr = baseAddr;
            Population = population;
            Connected = connected;
            Timestamp = timestamp;
            NnLowWatermark = nnLowWatermark;
            Depth = depth;
            Bins = bins;
        }

        public string BaseAddr { get; set; }

        public int Population { get; set; }

        public int Connected { get; set; }

        public string Timestamp { get; set; }

        public int NnLowWatermark { get; set; }

        public int Depth { get; set; }

        public IDictionary<string, AnonymousDto> Bins { get; set; }
    }
}

#pragma warning restore CA2227