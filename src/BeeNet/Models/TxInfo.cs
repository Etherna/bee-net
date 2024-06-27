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
    public sealed class TxInfo
    {
        // Constructors.
        internal TxInfo(Clients.PendingTransactions tx)
        {
            ArgumentNullException.ThrowIfNull(tx, nameof(tx));

            TransactionHash = tx.TransactionHash;
            To = tx.To;
            Nonce = tx.Nonce;
            GasPrice = XDaiBalance.FromWeiString(tx.GasPrice);
            GasLimit = tx.GasLimit;
            Data = tx.Data;
            Created = tx.Created;
            Description = tx.Description;
            Value = XDaiBalance.FromWeiString(tx.Value);
        }
        
        internal TxInfo(Clients.Response48 tx)
        {
            ArgumentNullException.ThrowIfNull(tx, nameof(tx));

            TransactionHash = tx.TransactionHash;
            To = tx.To;
            Nonce = tx.Nonce;
            GasPrice = XDaiBalance.FromWeiString(tx.GasPrice);
            GasLimit = tx.GasLimit;
            Data = tx.Data;
            Created = tx.Created;
            Description = tx.Description;
            Value = XDaiBalance.FromWeiString(tx.Value);
        }

        // Properties.
        public string TransactionHash { get; }
        public string To { get; }
        public int Nonce { get; }
        public XDaiBalance GasPrice { get; }
        public long GasLimit { get; }
        public string Data { get; }
        public DateTimeOffset Created { get; }
        public string Description { get; }
        public XDaiBalance Value { get; }
    }
}
