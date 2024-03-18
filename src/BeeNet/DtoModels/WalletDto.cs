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
    public class WalletDto
    {
        internal WalletDto(Clients.DebugApi.Response32 response32)
        {
            if (response32 is null)
                throw new ArgumentNullException(nameof(response32));

            Bzz = response32.BzzBalance;
            NativeTokenBalance = response32.NativeTokenBalance;
        }

        public string Bzz { get; }
        public string NativeTokenBalance { get; }
    }
}
