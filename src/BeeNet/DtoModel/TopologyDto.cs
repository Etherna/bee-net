using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class TopologyDto
    {
        public TopologyDto(Clients.v1_4.DebugApi.Response22 response22)
        {
            if (response22 is null)
            {
                return;
            }

            BaseAddr = response22.BaseAddr;
            Population = response22.Population;
            Connected = response22.Connected;
            Timestamp = response22.Timestamp;
            NnLowWatermark = response22.NnLowWatermark;
            Depth = response22.Depth;
            Bins = response22.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
        }

        public string BaseAddr { get; set; } = default!;
        
        public int Population { get; set; } = default!;
        
        public int Connected { get; set; } = default!;
        
        public string Timestamp { get; set; } = default!;
        
        public int NnLowWatermark { get; set; } = default!;
        
        public int Depth { get; set; } = default!;
        
        public IDictionary<string, AnonymousDto> Bins { get; set; } = default!;
    }
}
