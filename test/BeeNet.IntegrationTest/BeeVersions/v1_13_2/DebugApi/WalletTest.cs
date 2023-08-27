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
            var wallet = await beeNodeClient.DebugClient.GetWalletBalance();

            // Assert.
            Assert.NotEqual("0", wallet.Bzz);
            Assert.NotEqual("0", wallet.NativeTokenBalance);
        }

    }
}
