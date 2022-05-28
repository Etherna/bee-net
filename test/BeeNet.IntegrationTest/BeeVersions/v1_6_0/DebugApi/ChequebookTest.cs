﻿using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_0.DebugApi
{
    public class ChequebookTest : BaseTest_Debug_v2_0_1
    {
        [Fact]
        public async Task CashoutChequeForPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var result = await beeNodeClient.DebugClient.CashoutChequeForPeerAsync(peerId); //TODO this call return 500 "message": "cannot cash cheque"
            //TODO when return 500 there are some problems to deserialize the error message to DTO


            // Assert 
            Assert.StartsWith("0x", result);
        }

        [Fact]
        public async Task DepositIntoChequeBookAsync()
        {
            // Arrange 
            var originalChequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();
            var amount = 123;


            // Act 
            var result = await beeNodeClient.DebugClient.DepositIntoChequeBookAsync(amount);
            await Task.Delay(90000);


            // Assert 
            Assert.StartsWith("0x", result);
            var actualChequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();
            Assert.Equal(originalChequeBookBalance.AvailableBalance + amount, actualChequeBookBalance.AvailableBalance);
            Assert.Equal(originalChequeBookBalance.TotalBalance + amount, actualChequeBookBalance.TotalBalance);
        }

        [Fact]
        public async Task GetAllChequeBookChequesAsync()
        {
            // Arrange 
            var peer = "9356a84052cd4a1d5d9621bafc1fb4b05cba3cd0fd94e7085ee3998a245dc132";


            // Act 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();


            // Assert 
            Assert.Contains(allCheque, i => i.Peer == peer);
            var cheque = allCheque.First(i => i.Peer == peer);
            Assert.Equal(3151000000001, cheque.LastReceived.Payout);
        }

        [Fact]
        public async Task GetChequeBookAddressAsync()
        {
            // Arrange 


            // Act 
            var cheque = await beeNodeClient.DebugClient.GetChequeBookAddressAsync();


            // Assert 
            Assert.StartsWith("0x", cheque);
        }

        [Fact]
        public async Task GetChequeBookBalanceAsync()
        {
            // Arrange 


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();


            // Assert 
            Assert.True(chequeBookBalance.TotalBalance > 0);
            Assert.True(chequeBookBalance.AvailableBalance > 0);
        }

        [Fact]
        public async Task GetChequeBookCashoutForPeerAsync()
        {
            // Arrange 
            var peer = "9356a84052cd4a1d5d9621bafc1fb4b05cba3cd0fd94e7085ee3998a245dc132";


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookCashoutForPeerAsync(peer);


            // Assert 
            Assert.True(chequeBookBalance.UncashedAmount > 0);
            Assert.Equal(peer, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task GetChequeBookChequeForPeerAsync()
        {
            // Arrange 
            var peer = "9356a84052cd4a1d5d9621bafc1fb4b05cba3cd0fd94e7085ee3998a245dc132";


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookChequeForPeerAsync(peer);


            // Assert
            Assert.True(chequeBookBalance.LastReceived.Payout > 0);
            Assert.Equal(peer, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task WithdrawFromChequeBookAsync()
        {
            // Arrange 
            var originalChequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();
            var amount = 123;


            // Act 
            var result = await beeNodeClient.DebugClient.WithdrawFromChequeBookAsync(amount);
            await Task.Delay(90000);


            // Assert 
            Assert.StartsWith("0x", result);
            var actualChequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();
            Assert.Equal(originalChequeBookBalance.AvailableBalance - amount, actualChequeBookBalance.AvailableBalance);
            Assert.Equal(originalChequeBookBalance.TotalBalance - amount, actualChequeBookBalance.TotalBalance);
        }
    }
}
