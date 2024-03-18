﻿//   Copyright 2021-present Etherna SA
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
    public class TopologyDto
    {
        // Constructors.
        internal TopologyDto(Clients.DebugApi.Response22 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            BaseAddr = response.BaseAddr;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new PeersAggregateDto(i.Value));
            Connected = response.Connected;
            Depth = response.Depth;
            NetworkAvailability = response.NetworkAvailability switch
            {
                Clients.DebugApi.Response22NetworkAvailability.Unknown => NetworkAvailabilityDto.Unknown,
                Clients.DebugApi.Response22NetworkAvailability.Available => NetworkAvailabilityDto.Available,
                Clients.DebugApi.Response22NetworkAvailability.Unavailable => NetworkAvailabilityDto.Unavailable,
                _ => throw new InvalidOperationException(),
            };
            NnLowWatermark = response.NnLowWatermark;
            Population = response.Population;
            Reachability = response.Reachability switch
            {
                Clients.DebugApi.Response22Reachability.Unknown => ReachabilityDto.Unknown,
                Clients.DebugApi.Response22Reachability.Public => ReachabilityDto.Public,
                Clients.DebugApi.Response22Reachability.Private => ReachabilityDto.Private,
                _ => throw new InvalidOperationException(),
            };
            Timestamp = response.Timestamp;
        }

        internal TopologyDto(Clients.GatewayApi.Response37 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            BaseAddr = response.BaseAddr;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new PeersAggregateDto(i.Value));
            Connected = response.Connected;
            Depth = response.Depth;
            NetworkAvailability = response.NetworkAvailability switch
            {
                Clients.GatewayApi.Response37NetworkAvailability.Unknown => NetworkAvailabilityDto.Unknown,
                Clients.GatewayApi.Response37NetworkAvailability.Available => NetworkAvailabilityDto.Available,
                Clients.GatewayApi.Response37NetworkAvailability.Unavailable => NetworkAvailabilityDto.Unavailable,
                _ => throw new InvalidOperationException(),
            };
            NnLowWatermark = response.NnLowWatermark;
            Population = response.Population;
            Reachability = response.Reachability switch
            {
                Clients.GatewayApi.Response37Reachability.Unknown => ReachabilityDto.Unknown,
                Clients.GatewayApi.Response37Reachability.Public => ReachabilityDto.Public,
                Clients.GatewayApi.Response37Reachability.Private => ReachabilityDto.Private,
                _ => throw new InvalidOperationException(),
            };
            Timestamp = response.Timestamp;
        }

        // Properties.
        public string BaseAddr { get; }
        public IDictionary<string, PeersAggregateDto> Bins { get; }
        public int Connected { get; }
        public int Depth { get; }
        public NetworkAvailabilityDto NetworkAvailability { get; }
        public int NnLowWatermark { get; }
        public int Population { get; }
        public ReachabilityDto Reachability { get; }
        public string Timestamp { get; }
    }
}
