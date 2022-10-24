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
            // Arrange 


            // Act 
            var wallets = await beeNodeClient.DebugClient.GetWalletBalance();


            // Assert
            Assert.StartsWith("9", wallets.Bzz);
            Assert.StartsWith("9", wallets.XDai);
        }

    }
}
