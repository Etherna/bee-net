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
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public sealed class SettlementData
    {
        // Constructors.
        internal SettlementData(Clients.DebugApi.Settlements settlements)
        {
            ArgumentNullException.ThrowIfNull(settlements, nameof(settlements));

            Peer = settlements.Peer;
            Received = Convert.ToInt64(settlements.Received, CultureInfo.InvariantCulture);
            Sent = Convert.ToInt64(settlements.Sent, CultureInfo.InvariantCulture);
        }

        internal SettlementData(Clients.DebugApi.Settlements2 settlements)
        {
            ArgumentNullException.ThrowIfNull(settlements, nameof(settlements));

            Peer = settlements.Peer;
            Received = Convert.ToInt64(settlements.Received, CultureInfo.InvariantCulture);
            Sent = Convert.ToInt64(settlements.Sent, CultureInfo.InvariantCulture);
        }

        internal SettlementData(Clients.DebugApi.Response19 settlement)
        {
            ArgumentNullException.ThrowIfNull(settlement, nameof(settlement));

            Peer = settlement.Peer;
            Received = Convert.ToInt64(settlement.Received, CultureInfo.InvariantCulture);
            Sent = Convert.ToInt64(settlement.Sent, CultureInfo.InvariantCulture);
        }

        internal SettlementData(Clients.GatewayApi.Settlements settlement)
        {
            ArgumentNullException.ThrowIfNull(settlement, nameof(settlement));

            Peer = settlement.Peer;
            Received = Convert.ToInt64(settlement.Received, CultureInfo.InvariantCulture);
            Sent = Convert.ToInt64(settlement.Sent, CultureInfo.InvariantCulture);
        }

        internal SettlementData(Clients.GatewayApi.Settlements2 settlements)
        {
            ArgumentNullException.ThrowIfNull(settlements, nameof(settlements));

            Peer = settlements.Peer;
            Received = Convert.ToInt64(settlements.Received, CultureInfo.InvariantCulture);
            Sent = Convert.ToInt64(settlements.Sent, CultureInfo.InvariantCulture);
        }

        internal SettlementData(Clients.GatewayApi.Response35 settlement)
        {
            ArgumentNullException.ThrowIfNull(settlement, nameof(settlement));

            Peer = settlement.Peer;
            Received = Convert.ToInt64(settlement.Received, CultureInfo.InvariantCulture);
            Sent = Convert.ToInt64(settlement.Sent, CultureInfo.InvariantCulture);
        }

        // Properties.
        public string Peer { get; }
        public long Received { get; }
        public long Sent { get; }
    }
}
