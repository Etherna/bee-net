// Copyright 2021-present Etherna SA
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

using System;
using System.Collections.Generic;
using System.Globalization;
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
            Timestamp = DateTimeOffset.FromUnixTimeSeconds(
                long.Parse(response.Timestamp, CultureInfo.InvariantCulture));
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
        public DateTimeOffset Timestamp { get; }
    }
}
