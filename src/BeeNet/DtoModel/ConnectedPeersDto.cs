using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ConnectedPeersDto
    {
        // Constructors.
        public ConnectedPeersDto(Clients.v1_4.DebugApi.ConnectedPeers connectedPeers)
        {
            if (connectedPeers is null)
                throw new ArgumentNullException(nameof(connectedPeers));

            Address = connectedPeers.Address; 
            LastSeenTimestamp = connectedPeers.Metrics.LastSeenTimestamp;
            SessionConnectionRetry = connectedPeers.Metrics.SessionConnectionRetry;
            ConnectionTotalDuration = connectedPeers.Metrics.ConnectionTotalDuration;
            SessionConnectionDuration = connectedPeers.Metrics.SessionConnectionDuration;
            SessionConnectionDirection = connectedPeers.Metrics.SessionConnectionDirection;
            LatencyEWMA = connectedPeers.Metrics.LatencyEWMA;
        }


        // Properties.
        public string Address { get; }
        public int LastSeenTimestamp { get; }
        public int SessionConnectionRetry { get; }
        public double ConnectionTotalDuration { get; }
        public double SessionConnectionDuration { get; }
        public string SessionConnectionDirection { get; }
        public int LatencyEWMA { get; }
    }

}
