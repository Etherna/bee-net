﻿//   Copyright 2021-present Etherna Sagl
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
    public class ChequeBookCashoutGetDto
    {
        // Constructors.
        public ChequeBookCashoutGetDto(Clients.DebugApi.V3_0_2.Response26 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Peer = response.Peer;
            LastCashedCheque = response.LastCashedCheque is not null ? new LastCashedChequeDto(response.LastCashedCheque) : null;
            TransactionHash = response.TransactionHash;
            Result = response.Result is not null ? new ResultChequeBookDto(response.Result) : null;
            UncashedAmount = long.Parse(response.UncashedAmount, CultureInfo.InvariantCulture);
        }

        public ChequeBookCashoutGetDto(Clients.GatewayApi.V3_0_2.Response42 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Peer = response.Peer;
            LastCashedCheque = response.LastCashedCheque is not null ? new LastCashedChequeDto(response.LastCashedCheque) : null;
            TransactionHash = response.TransactionHash;
            Result = response.Result is not null ? new ResultChequeBookDto(response.Result) : null;
            UncashedAmount = long.Parse(response.UncashedAmount, CultureInfo.InvariantCulture);
        }

        // Properties.
        public string Peer { get; }
        public LastCashedChequeDto? LastCashedCheque { get; }
        public string TransactionHash { get; }
        public ResultChequeBookDto? Result { get; }
        public long UncashedAmount { get; }
    }

}
