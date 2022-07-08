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
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class SettlementDataDto
    {
        // Constructors.
        public SettlementDataDto(Clients.DebugApi.V2_0_1.Settlements settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = Convert.ToInt64(settlement.Received, CultureInfo.CurrentCulture);
            Sent = Convert.ToInt64(settlement.Sent, CultureInfo.CurrentCulture);
        }

        public SettlementDataDto(Clients.DebugApi.V2_0_1.Settlements2 settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = Convert.ToInt64(settlement.Received, CultureInfo.CurrentCulture);
            Sent = Convert.ToInt64(settlement.Sent, CultureInfo.CurrentCulture);
        }

        public SettlementDataDto(Clients.GatewayApi.V3_0_2.Settlements settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = settlement.Received;
            Sent = settlement.Sent;
        }

        public SettlementDataDto(Clients.GatewayApi.V3_0_2.Settlements2 settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = settlement.Received;
            Sent = settlement.Sent;
        }

        public SettlementDataDto(Clients.GatewayApi.V3_0_2.Response36 settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = Convert.ToInt64(settlement.Received, CultureInfo.CurrentCulture);
            Sent = Convert.ToInt64(settlement.Sent, CultureInfo.CurrentCulture);
        }

        // Properties.
        public string Peer { get; }
        public long Received { get; }
        public long Sent { get; }
    }
}
