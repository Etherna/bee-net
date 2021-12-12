using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class TopologyDto
    {
        // Constructors.
        public TopologyDto(Clients.v1_4.DebugApi.Response22 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Population = response.Population;
            Connected = response.Connected;
            Timestamp = response.Timestamp;
            NnLowWatermark = response.NnLowWatermark;
            Depth = response.Depth;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
        }


        // Properties.
        public string BaseAddr { get; }
        public int Population { get; }
        public int Connected { get; }
        public string Timestamp { get; }
        public int NnLowWatermark { get; }
        public int Depth { get; }
        public IDictionary<string, AnonymousDto> Bins { get; }
    }
}
