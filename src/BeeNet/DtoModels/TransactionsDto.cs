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
    public class TransactionsDto
    {
        // Constructors.
        internal TransactionsDto(Clients.DebugApi.V4_0_0.Response36 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
            To = response.To;
            Nonce = response.Nonce;
            GasPrice = long.Parse(response.GasPrice, CultureInfo.InvariantCulture);
            GasLimit = response.GasLimit;
            Data = response.Data;
            Created = response.Created;
            Description = response.Description;
            Value = long.Parse(response.Value, CultureInfo.InvariantCulture);
        }

        internal TransactionsDto(Clients.GatewayApi.V4_0_0.Response50 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
            To = response.To;
            Nonce = response.Nonce;
            GasPrice = long.Parse(response.GasPrice, CultureInfo.InvariantCulture);
            GasLimit = response.GasLimit;
            Data = response.Data;
            Created = response.Created;
            Description = response.Description;
            Value = long.Parse(response.Value, CultureInfo.InvariantCulture);
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
