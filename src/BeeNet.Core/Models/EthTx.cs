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

namespace Etherna.BeeNet.Models
{
    public sealed class EthTx(
        EthTxHash transactionHash,
        EthAddress to,
        int nonce,
        XDaiValue gasPrice,
        long gasLimit,
        string data,
        DateTimeOffset created,
        string description,
        XDaiValue value)
    {
        // Properties.
        public EthTxHash TransactionHash { get; } = transactionHash;
        public EthAddress To { get; } = to;
        public int Nonce { get; } = nonce;
        public XDaiValue GasPrice { get; } = gasPrice;
        public long GasLimit { get; } = gasLimit;
        public string Data { get; } = data;
        public DateTimeOffset Created { get; } = created;
        public string Description { get; } = description;
        public XDaiValue Value { get; } = value;
    }
}
