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
        public TopologyDto(Clients.DebugApi.V1_2_0.Response22 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Population = response.Population;
            Connected = response.Connected;
            Timestamp = response.Timestamp;
            NnLowWatermark = response.NnLowWatermark;
            Depth = response.Depth;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
        }

        public TopologyDto(Clients.DebugApi.V1_2_1.Response23 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Population = response.Population;
            Connected = response.Connected;
            Timestamp = response.Timestamp;
            NnLowWatermark = response.NnLowWatermark;
            Depth = response.Depth;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
        }
        public TopologyDto(Clients.DebugApi.V2_0_0.Response23 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Population = response.Population;
            Connected = response.Connected;
            Timestamp = response.Timestamp;
            NnLowWatermark = response.NnLowWatermark;
            Depth = response.Depth;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
        }

        public TopologyDto(Clients.DebugApi.V2_0_1.Response23 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            BaseAddr = response.BaseAddr;
            Population = response.Population;
            Connected = response.Connected;
            Timestamp = response.Timestamp;
            NnLowWatermark = response.NnLowWatermark;
            Depth = response.Depth;
            Bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
        }

        // Properties.
        public string BaseAddr { get; }
        public int Population { get; }
        public int Connected { get; }
        public string Timestamp { get; }
        public int NnLowWatermark { get; }
        public int Depth { get; }
        public IDictionary<string, AnonymousDto> Bins { get; }
    }
}
