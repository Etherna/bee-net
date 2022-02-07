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
    public class LastreceivedDto
    {
        // Constructors.
        public LastreceivedDto(Clients.v1_4_1.DebugApi.Lastreceived lastReceived)
        {
            if (lastReceived is null)
                throw new ArgumentNullException(nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = lastReceived.Payout;
        }

        public LastreceivedDto(Clients.v1_4_1.DebugApi.Lastreceived2 lastReceived)
        {
            if (lastReceived is null)
                throw new ArgumentNullException(nameof(lastReceived));

            Beneficiary = lastReceived.Beneficiary;
            Chequebook = lastReceived.Chequebook;
            Payout = lastReceived.Payout;
        }


        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Payout { get; }
    }
}
