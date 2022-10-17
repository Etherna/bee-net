//   Copyright 2021-present Etherna Sagl
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
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class ChequeBookBalanceDto
    {
        // Constructors.
        public ChequeBookBalanceDto(Clients.DebugApi.V3_2_0.Response9 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TotalBalance = long.Parse(response.TotalBalance, CultureInfo.InvariantCulture);
            AvailableBalance = long.Parse(response.AvailableBalance, CultureInfo.InvariantCulture);
        }

        public ChequeBookBalanceDto(Clients.GatewayApi.V3_2_0.Response27 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TotalBalance = long.Parse(response.TotalBalance, CultureInfo.InvariantCulture);
            AvailableBalance = long.Parse(response.AvailableBalance, CultureInfo.InvariantCulture);
        }

        // Properties.
        public long TotalBalance { get; }
        public long AvailableBalance { get; }
    }
}
