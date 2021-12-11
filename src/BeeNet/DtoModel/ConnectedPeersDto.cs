namespace Etherna.BeeNet.DtoModel
{
    public class ConnectedPeersDto
    {

        public ConnectedPeersDto(Clients.v1_4.DebugApi.ConnectedPeers connectedPeers)
        {
            if (connectedPeers is null)
            {
                return;
            }
            Address = connectedPeers.Address;

            if (connectedPeers.Metrics is not null)
            {
                LastSeenTimestamp = connectedPeers.Metrics.LastSeenTimestamp;
                SessionConnectionRetry = connectedPeers.Metrics.SessionConnectionRetry;
                ConnectionTotalDuration = connectedPeers.Metrics.ConnectionTotalDuration;
                SessionConnectionDuration = connectedPeers.Metrics.SessionConnectionDuration;
                SessionConnectionDirection = connectedPeers.Metrics.SessionConnectionDirection;
                LatencyEWMA = connectedPeers.Metrics.LatencyEWMA;
            }
        }

        public string Address { get; set; } = default!;
        public int LastSeenTimestamp { get; set; } = default!;
        public int SessionConnectionRetry { get; set; } = default!;
        public double ConnectionTotalDuration { get; set; } = default!;
        public double SessionConnectionDuration { get; set; } = default!;
        public string SessionConnectionDirection { get; set; } = default!;
        public int LatencyEWMA { get; set; } = default!;
    }

}
