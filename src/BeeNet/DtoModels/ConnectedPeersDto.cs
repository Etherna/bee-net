﻿//   Copyright 2021-present Etherna Sagl
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
    public class ConnectedPeersDto
    {
        // Constructors.
        public ConnectedPeersDto(Clients.DebugApi.V1_2_0.ConnectedPeers connectedPeers)
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

        public ConnectedPeersDto(Clients.DebugApi.V1_2_1.ConnectedPeers connectedPeers)
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

        public ConnectedPeersDto(Clients.DebugApi.V2_0_0.ConnectedPeers connectedPeers)
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
