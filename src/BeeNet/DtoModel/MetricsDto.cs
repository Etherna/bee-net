using System;

namespace Etherna.BeeNet.DtoModel
{
    public class MetricsDto
    {
        // Constructors.
        public MetricsDto(Clients.v1_4_1.DebugApi.Metrics metrics)
        {
            if (metrics is null)
                throw new ArgumentNullException(nameof(metrics));

            LastSeenTimestamp = metrics.LastSeenTimestamp;
            SessionConnectionRetry = metrics.SessionConnectionRetry;
            ConnectionTotalDuration = metrics.ConnectionTotalDuration;
            SessionConnectionDuration = metrics.SessionConnectionDuration;
            SessionConnectionDirection = metrics.SessionConnectionDirection;
            LatencyEWMA = metrics.LatencyEWMA;
        }


        // Properties.
        public int LastSeenTimestamp { get; }
        public int SessionConnectionRetry { get; }
        public double ConnectionTotalDuration { get; }
        public double SessionConnectionDuration { get; }
        public string SessionConnectionDirection { get; }
        public int LatencyEWMA { get; }
    }
}
