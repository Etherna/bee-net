namespace Etherna.BeeNet.DtoModel
{
    public class DisconnectedPeersDto
    {
        public DisconnectedPeersDto(Clients.v1_4.DebugApi.DisconnectedPeers disconnectedPeers)
        {
            if (disconnectedPeers is null)
            {
                return;
            }

            Address = disconnectedPeers.Address;
            Metrics = new MetricsDto(disconnectedPeers.Metrics);
        }
        
        public string Address { get; set; } = default!;
        
        public MetricsDto Metrics { get; set; } = default!;
    }
}
