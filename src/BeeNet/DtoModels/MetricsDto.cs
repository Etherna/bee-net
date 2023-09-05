//   Copyright 2021-present Etherna Sagl
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;

namespace Etherna.BeeNet.DtoModels
{
    public class MetricsDto
    {
        // Constructors
        internal MetricsDto(Clients.DebugApi.V5_0_0.Metrics metrics)
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

        internal MetricsDto(Clients.GatewayApi.V5_0_0.Metrics metrics)
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
