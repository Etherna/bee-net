// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

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
