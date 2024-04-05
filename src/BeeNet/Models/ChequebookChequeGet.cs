﻿//   Copyright 2021-present Etherna SA
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

namespace Etherna.BeeNet.Models
{
    public sealed class ChequebookChequeGet
    {
        // Constructors
        internal ChequebookChequeGet(Clients.DebugApi.Lastcheques response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceived(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSent(response.Lastsent) : null;
        }

        internal ChequebookChequeGet(Clients.DebugApi.Response27 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceived(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSent(response.Lastsent) : null;
        }

        internal ChequebookChequeGet(Clients.GatewayApi.Lastcheques response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceived(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSent(response.Lastsent) : null;
        }

        internal ChequebookChequeGet(Clients.GatewayApi.Response43 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Peer = response.Peer;
            LastReceived = response.Lastreceived is not null ? new LastReceived(response.Lastreceived) : null;
            LastSent = response.Lastsent is not null ? new LastSent(response.Lastsent) : null;
        }

        // Properties.
        public string Peer { get; }
        public LastReceived? LastReceived { get; }
        public LastSent? LastSent { get; }
    }
}