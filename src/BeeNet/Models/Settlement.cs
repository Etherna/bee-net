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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public sealed class Settlement
    {
        // Constructors.
        internal Settlement(Clients.Response36 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            TotalReceived = BzzBalance.FromPlurString(response.TotalReceived);
            TotalSent = BzzBalance.FromPlurString(response.TotalSent);
            Settlements = response.Settlements
                .Select(i => new SettlementData(i));
        }

        internal Settlement(Clients.Response37 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            TotalReceived = BzzBalance.FromPlurString(response.TotalReceived);
            TotalSent = BzzBalance.FromPlurString(response.TotalSent);
            Settlements = response.Settlements
                .Select(i => new SettlementData(i));
        }

        // Properties.
        public BzzBalance TotalReceived { get; }
        public BzzBalance TotalSent { get; }
        public IEnumerable<SettlementData> Settlements { get; }
    }
}
