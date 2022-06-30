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

namespace Etherna.BeeNet.DtoModels
{
    public class ChequeBookChequeGetDto
    {
        // Constructors.
        public ChequeBookChequeGetDto(Clients.GatewayApi.V3_0_2.Lastcheques response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Peer = response.Peer;
            LastReceived = new LastReceivedDto(response.Lastreceived);
            LastSent = response.Lastsent is not null ? new LastSentDto(response.Lastsent) : null;
        }

        public ChequeBookChequeGetDto(Clients.GatewayApi.V3_0_2.Response44 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Peer = response.Peer;
            LastReceived = new LastReceivedDto(response.Lastreceived);
            LastSent = response.Lastsent is not null ? new LastSentDto(response.Lastsent) : null;
        }

        // Properties.
        public string Peer { get; }
        public LastReceivedDto LastReceived { get; }
        public LastSentDto? LastSent { get; }
    }
}
