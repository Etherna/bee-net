﻿using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.DebugApi
{
    public class SettlementsTest : BaseTest_Debug_v2_0_1
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
            Assert.Contains(reserveState, i => i.Peer == peerId);
        }

    }
}
