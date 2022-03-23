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
    public class ResultChequeBookDto
    {
        // Constructors.
        public ResultChequeBookDto(Clients.DebugApi.V1_2_0.Result result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            Recipient = result.Recipient;
            LastPayout = long.Parse(result.LastPayout, CultureInfo.InvariantCulture);
            Bounced = result.Bounced;
        }

        public ResultChequeBookDto(Clients.DebugApi.V1_2_1.Result result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            Recipient = result.Recipient;
            LastPayout = long.Parse(result.LastPayout, CultureInfo.InvariantCulture);
            Bounced = result.Bounced;
        }

        public ResultChequeBookDto(Clients.DebugApi.V2_0_0.Result result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            Recipient = result.Recipient;
            LastPayout = long.Parse(result.LastPayout, CultureInfo.InvariantCulture);
            Bounced = result.Bounced;
        }

        // Properties.
        public bool Bounced { get; }
        public long LastPayout { get; }
        public string Recipient { get; }
    }
}
