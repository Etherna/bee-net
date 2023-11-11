//   Copyright 2021-present Etherna SA
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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModels
{
    public class PeersAggregateDto
    {
        // Constructors.
        internal PeersAggregateDto(Clients.DebugApi.V5_0_0.Anonymous2 anonymous)
        {
            if (anonymous is null)
                throw new ArgumentNullException(nameof(anonymous));

            Population = anonymous.Population;
            Connected = anonymous.Connected;
            DisconnectedPeers = anonymous.DisconnectedPeers
                ?.Select(k => new DisconnectedPeersDto(k)) ?? new List<DisconnectedPeersDto>();
            ConnectedPeers = anonymous.ConnectedPeers
                ?.Select(k => new ConnectedPeersDto(k)) ?? new List<ConnectedPeersDto>();
        }
        internal PeersAggregateDto(Clients.GatewayApi.V5_0_0.Anonymous2 anonymous)
        {
            if (anonymous is null)
                throw new ArgumentNullException(nameof(anonymous));

            Population = anonymous.Population;
            Connected = anonymous.Connected;
            DisconnectedPeers = anonymous.DisconnectedPeers
                ?.Select(k => new DisconnectedPeersDto(k)) ?? new List<DisconnectedPeersDto>();
            ConnectedPeers = anonymous.ConnectedPeers
                ?.Select(k => new ConnectedPeersDto(k)) ?? new List<ConnectedPeersDto>();
        }

        // Properties.
        public int Population { get; }
        public int Connected { get; }
        public IEnumerable<DisconnectedPeersDto> DisconnectedPeers { get; }
        public IEnumerable<ConnectedPeersDto> ConnectedPeers { get; }
    }
}
