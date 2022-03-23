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
    public class PendingTransactionDto
    {
        // Constructors.
        public PendingTransactionDto(Clients.DebugApi.V1_2_0.PendingTransactions tx)
        {
            if (tx is null)
                throw new ArgumentNullException(nameof(tx));

            TransactionHash = tx.TransactionHash;
            To = tx.To;
            Nonce = tx.Nonce;
            GasPrice = long.Parse(tx.GasPrice, CultureInfo.InvariantCulture);
            GasLimit = tx.GasLimit;
            Data = tx.Data;
            Created = tx.Created;
            Description = tx.Description;
            Value = long.Parse(tx.Value, CultureInfo.InvariantCulture);
        }

        public PendingTransactionDto(Clients.DebugApi.V1_2_1.PendingTransactions tx)
        {
            if (tx is null)
                throw new ArgumentNullException(nameof(tx));

            TransactionHash = tx.TransactionHash;
            To = tx.To;
            Nonce = tx.Nonce;
            GasPrice = long.Parse(tx.GasPrice, CultureInfo.InvariantCulture);
            GasLimit = tx.GasLimit;
            Data = tx.Data;
            Created = tx.Created;
            Description = tx.Description;
            Value = long.Parse(tx.Value, CultureInfo.InvariantCulture);
        }

        public PendingTransactionDto(Clients.DebugApi.V2_0_0.PendingTransactions tx)
        {
            if (tx is null)
                throw new ArgumentNullException(nameof(tx));

            TransactionHash = tx.TransactionHash;
            To = tx.To;
            Nonce = tx.Nonce;
            GasPrice = long.Parse(tx.GasPrice, CultureInfo.InvariantCulture);
            GasLimit = tx.GasLimit;
            Data = tx.Data;
            Created = tx.Created;
            Description = tx.Description;
            Value = long.Parse(tx.Value, CultureInfo.InvariantCulture);
        }

        // Properties.
        public string TransactionHash { get; }
        public string To { get; }
        public int Nonce { get; }
        public long GasPrice { get; }
        public long GasLimit { get; }
        public string Data { get; }
        public DateTimeOffset Created { get; }
        public string Description { get; }
        public long Value { get; }
    }
}
