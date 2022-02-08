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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class SettlementDto
    {
        // Constructors.
        public SettlementDto(Clients.DebugApi.v1_2_0.Response20 response20)
        {
            if (response20 is null)
                throw new ArgumentNullException(nameof(response20));

            TotalReceived = response20.TotalReceived;
            TotalSent = response20.TotalSent;
            Settlements = response20.Settlements
                .Select(i => new SettlementDataDto(i));
        }


        // Properties.
        public int TotalReceived { get; }
        public int TotalSent { get; }
        public IEnumerable<SettlementDataDto> Settlements { get; }
    }
}
