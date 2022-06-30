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
    public class BalanceDto
    {
        // Constructors.
        public BalanceDto(Clients.GatewayApi.V3_0_2.Balances balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.GatewayApi.V3_0_2.Balances2 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.GatewayApi.V3_0_2.Response21 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.GatewayApi.V3_0_2.Response24 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V1_2_0.Response3 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V1_2_1.Response3 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V2_0_0.Response3 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V2_0_1.Response3 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V1_2_0.Response6 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V1_2_1.Response6 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V2_0_0.Response6 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        public BalanceDto(Clients.DebugApi.V2_0_1.Response6 balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }


        // Properties.
        public string Peer { get; }
        public long Balance { get; }
    }
}
