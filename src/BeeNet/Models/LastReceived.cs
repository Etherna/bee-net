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

namespace Etherna.BeeNet.Models
{
    public sealed class LastReceived
    {
        // Constructors.
        internal LastReceived(Clients.GatewayApi.Lastreceived lastReceived)
        {
            ArgumentNullException.ThrowIfNull(lastReceived, nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = long.Parse(lastReceived.Payout, CultureInfo.InvariantCulture);
        }

        internal LastReceived(Clients.GatewayApi.Lastreceived2 lastReceived)
        {
            ArgumentNullException.ThrowIfNull(lastReceived, nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = long.Parse(lastReceived.Payout, CultureInfo.InvariantCulture);
        }

        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        public long Payout { get; }
    }
}
