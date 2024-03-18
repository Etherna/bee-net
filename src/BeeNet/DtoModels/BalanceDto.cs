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
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class BalanceDto
    {
        // Constructors.
        internal BalanceDto(Clients.DebugApi.Balances balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        internal BalanceDto(Clients.DebugApi.Balances2 balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        internal BalanceDto(Clients.DebugApi.Response4 balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        internal BalanceDto(Clients.DebugApi.Response7 balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        internal BalanceDto(Clients.GatewayApi.Balances balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        internal BalanceDto(Clients.GatewayApi.Balances2 balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        internal BalanceDto(Clients.GatewayApi.Response22 balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        internal BalanceDto(Clients.GatewayApi.Response24 balance)
        {
            ArgumentNullException.ThrowIfNull(balance, nameof(balance));

            Peer = balance.Peer;
            Balance = long.Parse(balance.Balance, CultureInfo.InvariantCulture);
        }

        // Properties.
        public string Peer { get; }
        public long Balance { get; }
    }
}
