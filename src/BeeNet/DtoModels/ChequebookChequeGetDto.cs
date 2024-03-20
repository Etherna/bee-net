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

namespace Etherna.BeeNet.DtoModels
{
    public class ChequeBookChequeGetDto
    {
        // Constructors
        internal ChequeBookChequeGetDto(Clients.DebugApi.Lastcheques response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceivedDto(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSentDto(response.Lastsent) : null;
        }

        internal ChequeBookChequeGetDto(Clients.DebugApi.Response27 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceivedDto(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSentDto(response.Lastsent) : null;
        }

        internal ChequeBookChequeGetDto(Clients.GatewayApi.Lastcheques response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceivedDto(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSentDto(response.Lastsent) : null;
        }

        internal ChequeBookChequeGetDto(Clients.GatewayApi.Response43 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceivedDto(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSentDto(response.Lastsent) : null;
        }

        // Properties.
        public string Peer { get; }
        public LastReceivedDto? LastReceived { get; }
        public LastSentDto? LastSent { get; }
    }
}
