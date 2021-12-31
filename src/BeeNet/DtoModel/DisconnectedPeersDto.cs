using System;

namespace Etherna.BeeNet.DtoModel
{
    public class DisconnectedPeersDto
    {
        // Constructors.
        public DisconnectedPeersDto(Clients.v1_4_1.DebugApi.DisconnectedPeers disconnectedPeers)
        {
            if (disconnectedPeers is null)
                throw new ArgumentNullException(nameof(disconnectedPeers));

            Address = disconnectedPeers.Address;
            Metrics = new MetricsDto(disconnectedPeers.Metrics);
        }


        // Properties.
        public string Address { get; }
        public MetricsDto Metrics { get; }
    }
}
