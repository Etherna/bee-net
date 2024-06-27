// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class ChequebookTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task CashoutChequeForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;

            // Act 
            var result = await beeNodeClient.CashoutChequeForPeerAsync(peerId);

            // Assert 
            Assert.StartsWith("0x", result);

            // Wait for avoid interferences with next tests.
            await Task.Delay(180000);
        }

        [Fact]
        public async Task DepositIntoChequeBookAsync()
        {
            // Arrange 
            var originalChequeBookBalance = await beeNodeClient.GetChequeBookBalanceAsync();
            var amount = 123;


            // Act 
            var result = await beeNodeClient.DepositIntoChequeBookAsync(amount);
            await Task.Delay(180000);


            // Assert 
            Assert.StartsWith("0x", result);
            var actualChequeBookBalance = await beeNodeClient.GetChequeBookBalanceAsync();
            Assert.Equal(originalChequeBookBalance.AvailableBalance + amount, actualChequeBookBalance.AvailableBalance);
            Assert.Equal(originalChequeBookBalance.TotalBalance + amount, actualChequeBookBalance.TotalBalance);
        }

        [Fact]
        public async Task GetAllChequeBookChequesAsync()
        {
            // Arrange 
            var allCheques = await beeNodeClient.GetAllChequeBookChequesAsync();
            var peerId = allCheques.ToList().First().Peer;


            // Act 
            var allCheque = await beeNodeClient.GetAllChequeBookChequesAsync(); //TODO this call return only one peer


            // Assert 
            Assert.Contains(allCheque, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetChequeBookAddressAsync()
        {
            // Arrange 


            // Act 
            var cheque = await beeNodeClient.GetChequeBookAddressAsync();


            // Assert 
            Assert.StartsWith("0x", cheque);
        }

        [Fact]
        public async Task GetChequeBookBalanceAsync()
        {
            // Arrange 

            // Act 
            await beeNodeClient.GetChequeBookBalanceAsync();
        }

        [Fact]
        public async Task GetChequeBookCashoutForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var chequeBookBalance = await beeNodeClient.GetChequeBookCashoutForPeerAsync(peerId);


            // Assert 
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task GetChequeBookChequeForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var chequeBookBalance = await beeNodeClient.GetChequeBookChequeForPeerAsync(peerId);


            // Assert
            Assert.True(chequeBookBalance.LastReceived.Payout > 0);
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task WithdrawFromChequeBookAsync()
        {
            // Arrange 
            var amount = 123;
            await beeNodeClient.DepositIntoChequeBookAsync(amount + 10);
            await Task.Delay(180000);
            var originalChequeBookBalance = await beeNodeClient.GetChequeBookBalanceAsync();


            // Act 
            var result = await beeNodeClient.WithdrawFromChequeBookAsync(amount);
            await Task.Delay(180000);


            // Assert 
            Assert.StartsWith("0x", result);
            var actualChequeBookBalance = await beeNodeClient.GetChequeBookBalanceAsync();
            Assert.Equal(originalChequeBookBalance.AvailableBalance - amount, actualChequeBookBalance.AvailableBalance);
            Assert.Equal(originalChequeBookBalance.TotalBalance - amount, actualChequeBookBalance.TotalBalance);
        }
    }
}
