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
    public class SettlementDataDto
    {
        // Constructors.
        public SettlementDataDto(Clients.v1_4_1.DebugApi.Settlements settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = settlement.Received;
            Sent = settlement.Sent;
        }

        public SettlementDataDto(Clients.v1_4_1.DebugApi.Settlements2 settlements)
        {
            if (settlements is null)
                throw new ArgumentNullException(nameof(settlements));

            Peer = settlements.Peer;
            Received = settlements.Received;
            Sent = settlements.Sent;
        }


        // Properties.
        public string Peer { get; }
        public int Received { get; }
        public int Sent { get; }
    }
}
