using System.Collections.Generic;

namespace Etherna.BeeNet.DtoModel
{
    public class AnonymousDto
    {
        public AnonymousDto(
            int population, 
            int connected, 
            ICollection<DisconnectedPeersDto> disconnectedPeers, 
            ICollection<ConnectedPeersDto> connectedPeers)
        {
            Population = population;
            Connected = connected;
            DisconnectedPeers = disconnectedPeers;
            ConnectedPeers = connectedPeers;
        }

        public int Population { get; set; } = default!;
        
        public int Connected { get; set; } = default!;
        
        public ICollection<DisconnectedPeersDto> DisconnectedPeers { get; set; } = default!;
        
        public ICollection<ConnectedPeersDto> ConnectedPeers { get; set; } = default!;
    }
}
