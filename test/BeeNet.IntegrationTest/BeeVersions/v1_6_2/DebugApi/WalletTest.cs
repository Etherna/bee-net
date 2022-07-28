using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_2.DebugApi
{
    public class WalletTest : BaseTest_Debug_v2_0_1
    {

        [Fact]
        public async Task GetWalletBalance()
        {
            // Arrange 


            // Act 
            var wallets = await beeNodeClient.DebugClient.GetWalletBalance();


            // Assert
            Assert.StartsWith("9", wallets.Bzz);
            Assert.StartsWith("9", wallets.XDai);
        }

    }
}
