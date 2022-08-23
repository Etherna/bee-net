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
        public TopologyDto(Clients.DebugApi.V3_0_2.Response23 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
            Connected = response.Connected;
            Depth = response.Depth;
            NetworkAvailability = response.NetworkAvailability switch
            {
                Clients.DebugApi.V3_0_2.Response23NetworkAvailability.Unknown => NetworkAvailabilityDto.Unknown,
                Clients.DebugApi.V3_0_2.Response23NetworkAvailability.Available => NetworkAvailabilityDto.Available,
                Clients.DebugApi.V3_0_2.Response23NetworkAvailability.Unavailable => NetworkAvailabilityDto.Unavailable,
                _ => throw new InvalidOperationException(),
            };
            NnLowWatermark = response.NnLowWatermark;
            Population = response.Population;
            Reachability = response.Reachability switch
            {
                Clients.DebugApi.V3_0_2.Response23Reachability.Unknown => ReachabilityDto.Unknown,
                Clients.DebugApi.V3_0_2.Response23Reachability.Public => ReachabilityDto.Public,
                Clients.DebugApi.V3_0_2.Response23Reachability.Private => ReachabilityDto.Private,
                _ => throw new InvalidOperationException(),
            };
            Timestamp = response.Timestamp;
        }

        public TopologyDto(Clients.GatewayApi.V3_0_2.Response39 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
            Connected = response.Connected;
            Depth = response.Depth;
            NetworkAvailability = response.NetworkAvailability switch
            {
                Clients.GatewayApi.V3_0_2.Response39NetworkAvailability.Unknown => NetworkAvailabilityDto.Unknown,
                Clients.GatewayApi.V3_0_2.Response39NetworkAvailability.Available => NetworkAvailabilityDto.Available,
                Clients.GatewayApi.V3_0_2.Response39NetworkAvailability.Unavailable => NetworkAvailabilityDto.Unavailable,
                _ => throw new InvalidOperationException(),
            };
            NnLowWatermark = response.NnLowWatermark;
            Population = response.Population;
            Reachability = response.Reachability switch
            {
                Clients.GatewayApi.V3_0_2.Response39Reachability.Unknown => ReachabilityDto.Unknown,
                Clients.GatewayApi.V3_0_2.Response39Reachability.Public => ReachabilityDto.Public,
                Clients.GatewayApi.V3_0_2.Response39Reachability.Private => ReachabilityDto.Private,
                _ => throw new InvalidOperationException(),
            };
            Timestamp = response.Timestamp;
        }

        // Properties.
        public string BaseAddr { get; }
        public IDictionary<string, AnonymousDto> Bins { get; }
        public int Connected { get; }
        public int Depth { get; }
        public NetworkAvailabilityDto NetworkAvailability { get; }
        public int NnLowWatermark { get; }
        public int Population { get; }
        public ReachabilityDto Reachability { get; }
        public string Timestamp { get; }
    }
}
