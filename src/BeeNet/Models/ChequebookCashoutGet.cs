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
            LastCashedCheque = response.LastCashedCheque is not null ? new ChequePayment(response.LastCashedCheque) : null;
            TransactionHash = response.TransactionHash;
            Result = new ResultChequebook(response.Result);
            UncashedAmount = BzzBalance.FromPlurString(response.UncashedAmount);
        }

        // Properties.
        public string Peer { get; }
        public ChequePayment? LastCashedCheque { get; }
        public string TransactionHash { get; }
        public ResultChequebook Result { get; }
        public BzzBalance UncashedAmount { get; }
    }

}
