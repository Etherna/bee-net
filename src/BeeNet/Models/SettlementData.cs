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
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public sealed class SettlementData
    {
        // Constructors.
        internal SettlementData(Clients.Settlements settlement)
        {
            ArgumentNullException.ThrowIfNull(settlement, nameof(settlement));

            Peer = settlement.Peer;
            Received = BzzBalance.FromPlurString(settlement.Received);
            Sent = BzzBalance.FromPlurString(settlement.Sent);
        }

        internal SettlementData(Clients.Settlements2 settlements)
        {
            ArgumentNullException.ThrowIfNull(settlements, nameof(settlements));

            Peer = settlements.Peer;
            Received = BzzBalance.FromPlurString(settlements.Received);
            Sent = BzzBalance.FromPlurString(settlements.Sent);
        }

        internal SettlementData(Clients.Response35 settlement)
        {
            ArgumentNullException.ThrowIfNull(settlement, nameof(settlement));

            Peer = settlement.Peer;
            Received = BzzBalance.FromPlurString(settlement.Received);
            Sent = BzzBalance.FromPlurString(settlement.Sent);
        }

        // Properties.
        public string Peer { get; }
        public BzzBalance Received { get; }
        public BzzBalance Sent { get; }
    }
}
