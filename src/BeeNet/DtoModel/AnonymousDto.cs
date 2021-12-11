using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class AnonymousDto
    {
        public AnonymousDto(Clients.v1_4.DebugApi.Anonymous anonymous)
        {
            if (anonymous is null)
            {
                return;
            }

            Population = anonymous.Population;
            Connected = anonymous.Connected;
            DisconnectedPeers = anonymous.DisconnectedPeers?
                .Select(k => new DisconnectedPeersDto(k))?.ToList();
            ConnectedPeers = anonymous.ConnectedPeers?
                .Select(k => new ConnectedPeersDto(k))?.ToList();
        }

        public int Population { get; set; } = default!;
        
        public int Connected { get; set; } = default!;
        
        public ICollection<DisconnectedPeersDto>? DisconnectedPeers { get; set; } = default!;
        
        public ICollection<ConnectedPeersDto>? ConnectedPeers { get; set; } = default!;
    }
}
