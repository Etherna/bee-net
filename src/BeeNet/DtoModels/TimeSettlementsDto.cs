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

namespace Etherna.BeeNet.DtoModels
{
    public class TimeSettlementsDto
    {
        // Constructors.
        internal TimeSettlementsDto(Clients.DebugApi.Response21 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            TotalReceived = Convert.ToInt64(response.TotalReceived, CultureInfo.InvariantCulture);
            TotalSent = Convert.ToInt64(response.TotalSent, CultureInfo.InvariantCulture);
            Settlements = response.Settlements
                .Select(i => new SettlementDataDto(i));
        }

        internal TimeSettlementsDto(Clients.GatewayApi.Response37 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            TotalReceived = Convert.ToInt64(response.TotalReceived, CultureInfo.InvariantCulture);
            TotalSent = Convert.ToInt64(response.TotalSent, CultureInfo.InvariantCulture);
            Settlements = response.Settlements
                .Select(i => new SettlementDataDto(i));
        }

        // Properties.
        public long TotalReceived { get; }
        public long TotalSent { get; }
        public IEnumerable<SettlementDataDto> Settlements { get; }
    }
}
