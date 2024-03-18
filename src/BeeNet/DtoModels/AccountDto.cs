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
    public class AccountDto
    {
        // Constructors.
        internal AccountDto(Clients.DebugApi.Anonymous anonymous)
        {
            if (anonymous is null)
                throw new ArgumentNullException(nameof(anonymous));

            Balance = anonymous.Balance;
            ThresholdReceived = anonymous.ThresholdReceived;
            ThresholdGiven = anonymous.ThresholdGiven;
            SurplusBalance = anonymous.SurplusBalance;
            ReservedBalance = anonymous.ReservedBalance;
            ShadowReservedBalance = anonymous.ShadowReservedBalance;
            GhostBalance = anonymous.GhostBalance;
        }

        // Properties.
        public string Balance { get; set; }
        public string ThresholdReceived { get; set; }
        public string ThresholdGiven { get; set; }
        public string SurplusBalance { get; set; }
        public string ReservedBalance { get; set; }
        public string ShadowReservedBalance { get; set; }
        public string GhostBalance { get; set; }
    }
}
