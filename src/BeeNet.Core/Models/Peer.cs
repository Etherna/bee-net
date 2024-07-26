﻿// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

namespace Etherna.BeeNet.Models
{
    public sealed class Peer(
        string address,
        int lastSeenTimestamp,
        int sessionConnectionRetry,
        double connectionTotalDuration,
        double sessionConnectionDuration,
        string sessionConnectionDirection,
        int latencyEwma)
    {
        // Properties.
        public string Address { get; } = address;
        public int LastSeenTimestamp { get; } = lastSeenTimestamp;
        public int SessionConnectionRetry { get; } = sessionConnectionRetry;
        public double ConnectionTotalDuration { get; } = connectionTotalDuration;
        public double SessionConnectionDuration { get; } = sessionConnectionDuration;
        public string SessionConnectionDirection { get; } = sessionConnectionDirection;
        public int LatencyEwma { get; } = latencyEwma;
    }

}
