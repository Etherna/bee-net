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

using Etherna.BeeNet.Clients;
using System;

namespace Etherna.BeeNet.Models
{
    public sealed class ChequePayment
    {
        // Constructors.
        internal ChequePayment(LastCashedCheque lastCashedCheque)
        {
            ArgumentNullException.ThrowIfNull(lastCashedCheque, nameof(lastCashedCheque));

            Beneficiary = lastCashedCheque.Beneficiary;
            Chequebook = lastCashedCheque.Chequebook;
            Payout = BzzBalance.FromPlurString(lastCashedCheque.Payout);
        }
        internal ChequePayment(Lastreceived lastReceived)
        {
            ArgumentNullException.ThrowIfNull(lastReceived, nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = BzzBalance.FromPlurString(lastReceived.Payout);
        }
        internal ChequePayment(Lastreceived2 lastReceived)
        {
            ArgumentNullException.ThrowIfNull(lastReceived, nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = BzzBalance.FromPlurString(lastReceived.Payout);
        }
        internal ChequePayment(Lastsent lastsent)
        {
            ArgumentNullException.ThrowIfNull(lastsent, nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = BzzBalance.FromPlurString(lastsent.Payout);
        }
        internal ChequePayment(Lastsent2 lastsent)
        {
            ArgumentNullException.ThrowIfNull(lastsent, nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = BzzBalance.FromPlurString(lastsent.Payout);
        }

        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        public BzzBalance Payout { get; }
    }
}
