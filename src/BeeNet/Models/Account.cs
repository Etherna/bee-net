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
    public sealed class Account
    {
        // Constructors.
        internal Account(Clients.DebugApi.Anonymous value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            Balance = value.Balance;
            ThresholdReceived = value.ThresholdReceived;
            ThresholdGiven = value.ThresholdGiven;
            SurplusBalance = value.SurplusBalance;
            ReservedBalance = value.ReservedBalance;
            ShadowReservedBalance = value.ShadowReservedBalance;
            GhostBalance = value.GhostBalance;
        }

        internal Account(Clients.GatewayApi.Anonymous3 value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            Balance = value.Balance;
            ThresholdReceived = value.ThresholdReceived;
            ThresholdGiven = value.ThresholdGiven;
            SurplusBalance = value.SurplusBalance;
            ReservedBalance = value.ReservedBalance;
            ShadowReservedBalance = value.ShadowReservedBalance;
            GhostBalance = value.GhostBalance;
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
