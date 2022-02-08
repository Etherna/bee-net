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
    public class ChequeBookChequeGetDto
    {
        // Constructors.
        public ChequeBookChequeGetDto(Clients.DebugApi.v1_2_0.Response27 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Peer = response.Peer;
            Lastreceived = new LastreceivedDto(response.Lastreceived);
            Lastsent = new LastsentDto(response.Lastsent);
        }

        public ChequeBookChequeGetDto(Clients.DebugApi.v1_2_0.Lastcheques lastcheques)
        {
            if (lastcheques is null)
                throw new ArgumentNullException(nameof(lastcheques));

            Peer = lastcheques.Peer;
            Lastreceived = new LastreceivedDto(lastcheques.Lastreceived);
            Lastsent = new LastsentDto(lastcheques.Lastsent);
        }


        // Properties.
        public string Peer { get; }
        public LastreceivedDto Lastreceived { get; }
        public LastsentDto Lastsent { get; }
    }
}
