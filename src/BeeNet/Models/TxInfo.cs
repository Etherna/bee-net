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
