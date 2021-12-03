#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class AnonymousDto : BaseDto
    {
        public AnonymousDto(
            int population, 
            int connected, 
            ICollection<DisconnectedPeersDto>? disconnectedPeers, 
            ICollection<ConnectedPeersDto>? connectedPeers, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Population = population;
            Connected = connected;
            DisconnectedPeers = disconnectedPeers;
            ConnectedPeers = connectedPeers;
        }

        public int Population { get; set; }

        public int Connected { get; set; }

        public ICollection<DisconnectedPeersDto>? DisconnectedPeers { get; set; }
        public ICollection<ConnectedPeersDto>? ConnectedPeers { get; set; }
    }
}

#pragma warning restore CA2227