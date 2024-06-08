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

namespace Etherna.BeeNet.Models
{
    public sealed class RedistributionState
    {
        // Constructors.
        internal RedistributionState(Clients.Response60 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            IsFrozen = response.IsFrozen;
            IsFullySynced = response.IsFullySynced;
            IsHealthy = response.IsHealthy;
            Round = response.Round;
            LastWonRound = response.LastWonRound;
            LastPlayedRound = response.LastPlayedRound;
            LastFrozenRound = response.LastFrozenRound;
            Block = response.Block;
            Reward = BzzBalance.FromPlurString(response.Reward);
            Fees = XDaiBalance.FromWeiString(response.Fees);
        }

        // Properties.
        public bool IsFrozen { get; set; }
        public bool IsFullySynced { get; set; }
        public bool IsHealthy { get; set; }
        public int Round { get; set; } 
        public int LastWonRound { get; set; } 
        public int LastPlayedRound { get; set; } 
        public int LastFrozenRound { get; set; } 
        public int Block { get; set; } 
        public BzzBalance Reward { get; set; }
        public XDaiBalance Fees { get; set; } 
    }
}
