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

namespace Etherna.BeeNet.DtoModels
{
    public class RedistributionStateDto
    {
        // Constructors.
        internal RedistributionStateDto(Clients.DebugApi.V5_0_0.Response31 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            IsFrozen = response.IsFrozen;
            IsFullySynced = response.IsFullySynced;
            Round = response.Round;
            LastWonRound = response.LastWonRound;
            LastPlayedRound = response.LastPlayedRound;
            LastFrozenRound = response.LastFrozenRound;
            Block = response.Block;
            Reward = response.Reward;
            Fees = response.Fees;
        }

        // Properties.
        public bool IsFrozen { get; set; }
        public bool IsFullySynced { get; set; }
        public int Round { get; set; } 
        public int LastWonRound { get; set; } 
        public int LastPlayedRound { get; set; } 
        public int LastFrozenRound { get; set; } 
        public int Block { get; set; } 
        public string Reward { get; set; }
        public string Fees { get; set; } 
    }
}
