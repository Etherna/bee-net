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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModels
{
    public class TopologyDto
    {
        // Constructors.
        internal TopologyDto(Clients.DebugApi.V4_0_0.Response22 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new PeersAggregateDto(i.Value));
            Connected = response.Connected;
            Depth = response.Depth;
            NetworkAvailability = response.NetworkAvailability switch
            {
                Clients.DebugApi.V4_0_0.Response22NetworkAvailability.Unknown => NetworkAvailabilityDto.Unknown,
                Clients.DebugApi.V4_0_0.Response22NetworkAvailability.Available => NetworkAvailabilityDto.Available,
                Clients.DebugApi.V4_0_0.Response22NetworkAvailability.Unavailable => NetworkAvailabilityDto.Unavailable,
                _ => throw new InvalidOperationException(),
            };
            NnLowWatermark = response.NnLowWatermark;
            Population = response.Population;
            Reachability = response.Reachability switch
            {
                Clients.DebugApi.V4_0_0.Response22Reachability.Unknown => ReachabilityDto.Unknown,
                Clients.DebugApi.V4_0_0.Response22Reachability.Public => ReachabilityDto.Public,
                Clients.DebugApi.V4_0_0.Response22Reachability.Private => ReachabilityDto.Private,
                _ => throw new InvalidOperationException(),
            };
            Timestamp = response.Timestamp;
        }

        internal TopologyDto(Clients.GatewayApi.V4_0_0.Response38 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new PeersAggregateDto(i.Value));
            Connected = response.Connected;
            Depth = response.Depth;
            NetworkAvailability = response.NetworkAvailability switch
            {
                Clients.GatewayApi.V4_0_0.Response38NetworkAvailability.Unknown => NetworkAvailabilityDto.Unknown,
                Clients.GatewayApi.V4_0_0.Response38NetworkAvailability.Available => NetworkAvailabilityDto.Available,
                Clients.GatewayApi.V4_0_0.Response38NetworkAvailability.Unavailable => NetworkAvailabilityDto.Unavailable,
                _ => throw new InvalidOperationException(),
            };
            NnLowWatermark = response.NnLowWatermark;
            Population = response.Population;
            Reachability = response.Reachability switch
            {
                Clients.GatewayApi.V4_0_0.Response38Reachability.Unknown => ReachabilityDto.Unknown,
                Clients.GatewayApi.V4_0_0.Response38Reachability.Public => ReachabilityDto.Public,
                Clients.GatewayApi.V4_0_0.Response38Reachability.Private => ReachabilityDto.Private,
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
