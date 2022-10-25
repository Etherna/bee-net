using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_9_0.DebugApi
{
    public class WalletTest : BaseTest_Debug_V3_2_0
    {

        [Fact]
        public async Task GetWalletBalance()
        {
            // Act.
            var wallet = await beeNodeClient.DebugClient.GetWalletBalance();

            // Assert.
            Assert.NotEqual(0, wallet.Bzz);
            Assert.NotEqual(0, wallet.XDai);
        }

    }
}
