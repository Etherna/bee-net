namespace Etherna.BeeNet.DtoModel
{
    public class ConnectedPeersDto
    {

        public ConnectedPeersDto(
            string address, 
            int lastSeenTimestamp, 
            int sessionConnectionRetry, 
            double connectionTotalDuration, 
            double sessionConnectionDuration, 
            string sessionConnectionDirection, 
            int latencyEwma)
        {
            Address = address;
            LastSeenTimestamp = lastSeenTimestamp;
            SessionConnectionRetry = sessionConnectionRetry;
            ConnectionTotalDuration = connectionTotalDuration;
            SessionConnectionDuration = sessionConnectionDuration;
            SessionConnectionDirection = sessionConnectionDirection;
            LatencyEWMA = latencyEwma;
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
