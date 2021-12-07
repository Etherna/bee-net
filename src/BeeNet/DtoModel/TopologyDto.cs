using System.Collections.Generic;

namespace Etherna.BeeNet.DtoModel
{
    public class TopologyDto
    {
        public TopologyDto(
            string baseAddr, 
            int population, 
            int connected, 
            string timestamp, 
            int nnLowWatermark, 
            int depth, 
            IDictionary<string, AnonymousDto> bins)
        {
            BaseAddr = baseAddr;
            Population = population;
            Connected = connected;
            Timestamp = timestamp;
            NnLowWatermark = nnLowWatermark;
            Depth = depth;
            Bins = bins;
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
