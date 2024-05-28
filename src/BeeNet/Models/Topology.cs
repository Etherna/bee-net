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

namespace Etherna.BeeNet.Models
{
    public sealed class Topology
    {
        // Constructors.
        internal Topology(Clients.Response38 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            BaseAddr = response.BaseAddr;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new PeersAggregate(i.Value));
            Connected = response.Connected;
            Depth = response.Depth;
            NetworkAvailability = response.NetworkAvailability switch
            {
                Clients.Response38NetworkAvailability.Unknown => Models.NetworkAvailability.Unknown,
                Clients.Response38NetworkAvailability.Available => Models.NetworkAvailability.Available,
                Clients.Response38NetworkAvailability.Unavailable => Models.NetworkAvailability.Unavailable,
                _ => throw new InvalidOperationException(),
            };
            NnLowWatermark = response.NnLowWatermark;
            Population = response.Population;
            Reachability = response.Reachability switch
            {
                Clients.Response38Reachability.Unknown => Models.Reachability.Unknown,
                Clients.Response38Reachability.Public => Models.Reachability.Public,
                Clients.Response38Reachability.Private => Models.Reachability.Private,
                _ => throw new InvalidOperationException(),
            };
            Timestamp = response.Timestamp;
        }

        // Properties.
        public string BaseAddr { get; }
        public IDictionary<string, PeersAggregate> Bins { get; }
        public int Connected { get; }
        public int Depth { get; }
        public NetworkAvailability NetworkAvailability { get; }
        public int NnLowWatermark { get; }
        public int Population { get; }
        public Reachability Reachability { get; }
        public string Timestamp { get; }
    }
}
