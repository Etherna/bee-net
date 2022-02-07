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

namespace Etherna.BeeNet.DtoModel
{
    public class TransactionHashDto
    {
        // Constructors.
        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response26 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response29 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response34 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response35 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }


        // Properties.
        public string TransactionHash { get; }
    }
}
