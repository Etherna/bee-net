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
    public sealed class RedistributionState(
        bool isFrozen,
        bool isFullySynced,
        bool isHealthy,
        int round,
        int lastWonRound,
        int lastPlayedRound,
        int lastFrozenRound,
        int block,
        BzzBalance reward,
        XDaiBalance fees)
    {
        // Properties.
        public bool IsFrozen { get; } = isFrozen;
        public bool IsFullySynced { get; } = isFullySynced;
        public bool IsHealthy { get; } = isHealthy;
        public int Round { get; } = round;
        public int LastWonRound { get; } = lastWonRound;
        public int LastPlayedRound { get; } = lastPlayedRound;
        public int LastFrozenRound { get; } = lastFrozenRound;
        public int Block { get; } = block;
        public BzzBalance Reward { get; } = reward;
        public XDaiBalance Fees { get; } = fees;
    }
}
