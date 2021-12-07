namespace Etherna.BeeNet.DtoModel
{
    public class MetricsDto
    {
        public MetricsDto(
            int lastSeenTimestamp, 
            int sessionConnectionRetry, 
            double connectionTotalDuration, 
            double sessionConnectionDuration, 
            string sessionConnectionDirection, 
            int latencyEwma)
        {
            LastSeenTimestamp = lastSeenTimestamp;
            SessionConnectionRetry = sessionConnectionRetry;
            ConnectionTotalDuration = connectionTotalDuration;
            SessionConnectionDuration = sessionConnectionDuration;
            SessionConnectionDirection = sessionConnectionDirection;
            LatencyEWMA = latencyEwma;
        }

        public int LastSeenTimestamp { get; set; } = default!;
        
        public int SessionConnectionRetry { get; set; } = default!;
        
        public double ConnectionTotalDuration { get; set; } = default!;
        
        public double SessionConnectionDuration { get; set; } = default!;
        
        public string SessionConnectionDirection { get; set; } = default!;
        
        public int LatencyEWMA { get; set; } = default!;
    }
}
