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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.DebugApi
{
    public class WalletTest : BaseTest_Debug_v5_0_0
    {

        [Fact]
        public async Task GetWalletBalance()
        {
            // Act.
            var wallet = await beeNodeClient.GatewayClient.GetWalletBalance();

            // Assert.
            Assert.NotEqual("0", wallet.Bzz);
            Assert.NotEqual("0", wallet.NativeTokenBalance);
        }

    }
}
