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

namespace Etherna.BeeNet.Models
{
    public sealed class Account
    {
        // Constructors.
        internal Account(Clients.Anonymous3 value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            Balance = BzzBalance.FromPlurString(value.Balance);
            ThresholdReceived = BzzBalance.FromPlurString(value.ThresholdReceived);
            ThresholdGiven = BzzBalance.FromPlurString(value.ThresholdGiven);
            SurplusBalance = BzzBalance.FromPlurString(value.SurplusBalance);
            ReservedBalance = BzzBalance.FromPlurString(value.ReservedBalance);
            ShadowReservedBalance = BzzBalance.FromPlurString(value.ShadowReservedBalance);
            GhostBalance = BzzBalance.FromPlurString(value.GhostBalance);
        }

        // Properties.
        public BzzBalance Balance { get; set; }
        public BzzBalance ThresholdReceived { get; set; }
        public BzzBalance ThresholdGiven { get; set; }
        public BzzBalance SurplusBalance { get; set; }
        public BzzBalance ReservedBalance { get; set; }
        public BzzBalance ShadowReservedBalance { get; set; }
        public BzzBalance GhostBalance { get; set; }
    }
}
