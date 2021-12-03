#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class MetricsDto : BaseDto
    {
        public MetricsDto(int lastSeenTimestamp, int sessionConnectionRetry, double connectionTotalDuration, double sessionConnectionDuration, string sessionConnectionDirection, int latencyEWMA, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            LastSeenTimestamp = lastSeenTimestamp;
            SessionConnectionRetry = sessionConnectionRetry;
            ConnectionTotalDuration = connectionTotalDuration;
            SessionConnectionDuration = sessionConnectionDuration;
            SessionConnectionDirection = sessionConnectionDirection;
            LatencyEWMA = latencyEWMA;
        }

        public int LastSeenTimestamp { get; set; }

        public int SessionConnectionRetry { get; set; }

        public double ConnectionTotalDuration { get; set; }

        public double SessionConnectionDuration { get; set; }

        public string SessionConnectionDirection { get; set; }

        public int LatencyEWMA { get; set; }
    }
}

#pragma warning restore CA2227