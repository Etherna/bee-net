﻿//   Copyright 2021-present Etherna Sagl
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

namespace Etherna.BeeNet.DtoModel
{
    public class PendingTransactionDto
    {
        // Constructors.
        public PendingTransactionDto(Clients.DebugApi.v1_2_0.PendingTransactions pendingTransactions)
        {
            if (pendingTransactions is null)
                throw new ArgumentNullException(nameof(pendingTransactions));

            TransactionHash = pendingTransactions.TransactionHash;
            To = pendingTransactions.To;
            Nonce = pendingTransactions.Nonce;
            GasPrice = pendingTransactions.GasPrice;
            GasLimit = pendingTransactions.GasLimit;
            Data = pendingTransactions.Data;
            Created = pendingTransactions.Created;
            Description = pendingTransactions.Description;
            Value = pendingTransactions.Value;
        }


        // Properties.
        public string TransactionHash { get; }
        public string To { get; }
        public int Nonce { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string GasPrice { get; }
        public int GasLimit { get; }
        public string Data { get; }
        public DateTimeOffset Created { get; }
        public string Description { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Value { get; }
    }
}
