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

namespace Etherna.BeeNet.DtoModel
{
    public class ChequeBookBalanceDto
    {
        // Constructors.
        public ChequeBookBalanceDto(Clients.v1_4_1.DebugApi.Response8 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TotalBalance = response.TotalBalance;
            AvailableBalance = response.AvailableBalance;
        }


        // Properties.
        public string TotalBalance { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string AvailableBalance { get; }
    }
}
