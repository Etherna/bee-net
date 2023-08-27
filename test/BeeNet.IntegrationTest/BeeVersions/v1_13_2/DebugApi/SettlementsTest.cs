using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.DebugApi
{
    public class SettlementsTest : BaseTest_Debug_v5_0_0
    {

        [Fact]
        public async Task GetAllSettlementsAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;

            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetAllSettlementsAsync();


            // Assert
            Assert.Contains(reserveState.Settlements, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetAllTimeSettlementsAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetAllTimeSettlementsAsync();


            // Assert
            Assert.Contains(reserveState.Settlements, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetSettlementsWithPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var reserveState = await beeNodeClient.DebugClient.GetSettlementsWithPeerAsync(peerId);


            // Assert
            Assert.Equal(peerId, reserveState.Peer);
        }

    }
}
