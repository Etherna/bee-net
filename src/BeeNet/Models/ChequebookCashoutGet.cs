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
    public sealed class ChequebookCashoutGet
    {
        // Constructors.
        internal ChequebookCashoutGet(Clients.Response41 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastCashedCheque = response.LastCashedCheque is not null ? new LastCashedCheque(response.LastCashedCheque) : null;
            TransactionHash = response.TransactionHash;
            Result = new ResultChequebook(response.Result);
            UncashedAmount = long.Parse(response.UncashedAmount, CultureInfo.InvariantCulture);
        }

        // Properties.
        public string Peer { get; }
        public LastCashedCheque? LastCashedCheque { get; }
        public string TransactionHash { get; }
        public ResultChequebook Result { get; }
        public long UncashedAmount { get; }
    }

}
