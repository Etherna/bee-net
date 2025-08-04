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

namespace Etherna.BeeNet.Models
{
    public sealed class Account(
        BzzValue balance,
        BzzValue thresholdReceived,
        BzzValue thresholdGiven,
        BzzValue surplusBalance,
        BzzValue reservedBalance,
        BzzValue shadowReservedBalance,
        BzzValue ghostBalance)
    {
        // Properties.
        public BzzValue Balance { get; } = balance;
        public BzzValue ThresholdReceived { get; } = thresholdReceived;
        public BzzValue ThresholdGiven { get; } = thresholdGiven;
        public BzzValue SurplusBalance { get; } = surplusBalance;
        public BzzValue ReservedBalance { get; } = reservedBalance;
        public BzzValue ShadowReservedBalance { get; } = shadowReservedBalance;
        public BzzValue GhostBalance { get; } = ghostBalance;
    }
}
