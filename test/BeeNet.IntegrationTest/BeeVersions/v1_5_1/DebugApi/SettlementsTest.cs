using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_5_1.DebugApi
{
    public class SettlementsTest : BaseTest_Debug_v2_0_0
    {
        [Fact]
        public async Task GetAllSettlementsAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetAllSettlementsAsync();


            // Assert
        }

        [Fact]
        public async Task GetAllTimeSettlementsAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetAllTimeSettlementsAsync();


            // Assert
        }

        [Fact]
        public async Task GetSettlementsWithPeerAsync()
        {
            // Arrange 


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetSettlementsWithPeerAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Assert
        }

    }
}
