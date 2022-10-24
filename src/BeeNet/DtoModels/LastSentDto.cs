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
    public class LastSentDto
    {
        // Constructors.
        public LastSentDto(Clients.DebugApi.V3_2_0.Lastsent lastsent)
        {
            if (lastsent is null)
                throw new ArgumentNullException(nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = long.Parse(lastsent.Payout, CultureInfo.InvariantCulture);
        }

        public LastSentDto(Clients.DebugApi.V3_2_0.Lastsent2 lastsent)
        {
            if (lastsent is null)
                throw new ArgumentNullException(nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = long.Parse(lastsent.Payout, CultureInfo.InvariantCulture);
        }

        public LastSentDto(Clients.GatewayApi.V3_2_0.Lastsent lastsent)
        {
            if (lastsent is null)
                throw new ArgumentNullException(nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = long.Parse(lastsent.Payout, CultureInfo.InvariantCulture);
        }

        public LastSentDto(Clients.GatewayApi.V3_2_0.Lastsent2 lastsent)
        {
            if (lastsent is null)
                throw new ArgumentNullException(nameof(lastsent));

            Beneficiary = lastsent.Beneficiary;
            Chequebook = lastsent.Chequebook;
            Payout = long.Parse(lastsent.Payout, CultureInfo.InvariantCulture);
        }

        // Properties.
        public string Beneficiary { get; }
        public string Chequebook { get; }
        public long Payout { get; }
    }
}
