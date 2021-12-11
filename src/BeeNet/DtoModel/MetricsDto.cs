namespace Etherna.BeeNet.DtoModel
{
    public class MetricsDto
    {
        public MetricsDto(Clients.v1_4.DebugApi.Metrics metrics)
        {
            if (metrics is null)
            {
                return;
            }

            LastSeenTimestamp = metrics.LastSeenTimestamp;
            SessionConnectionRetry = metrics.SessionConnectionRetry;
            ConnectionTotalDuration = metrics.ConnectionTotalDuration;
            SessionConnectionDuration = metrics.SessionConnectionDuration;
            SessionConnectionDirection = metrics.SessionConnectionDirection;
            LatencyEWMA = metrics.LatencyEWMA;
        }

        public int LastSeenTimestamp { get; set; } = default!;
        
        public int SessionConnectionRetry { get; set; } = default!;
        
        public double ConnectionTotalDuration { get; set; } = default!;
        
        public double SessionConnectionDuration { get; set; } = default!;
        
        public string SessionConnectionDirection { get; set; } = default!;
        
        public int LatencyEWMA { get; set; } = default!;
    }
}
