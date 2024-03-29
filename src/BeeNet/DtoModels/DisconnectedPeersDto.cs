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
    public class DisconnectedPeersDto
    {
        // Constructors.
        public DisconnectedPeersDto(Clients.DebugApi.V1_2_0.DisconnectedPeers disconnectedPeers)
        {
            if (disconnectedPeers is null)
                throw new ArgumentNullException(nameof(disconnectedPeers));

            Address = disconnectedPeers.Address;
            Metrics = new MetricsDto(disconnectedPeers.Metrics);
        }

        public DisconnectedPeersDto(Clients.DebugApi.V1_2_1.DisconnectedPeers disconnectedPeers)
        {
            if (disconnectedPeers is null)
                throw new ArgumentNullException(nameof(disconnectedPeers));

            Address = disconnectedPeers.Address;
            Metrics = new MetricsDto(disconnectedPeers.Metrics);
        }

        public DisconnectedPeersDto(Clients.DebugApi.V2_0_0.DisconnectedPeers disconnectedPeers)
        {
            if (disconnectedPeers is null)
                throw new ArgumentNullException(nameof(disconnectedPeers));

            Address = disconnectedPeers.Address;
            Metrics = new MetricsDto(disconnectedPeers.Metrics);
        }

        // Properties.
        public string Address { get; }
        public MetricsDto Metrics { get; }
    }
}
