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

namespace BeeNet.Client.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class ChequebookTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task CashoutChequeForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequebookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;

            // Act 
            var result = await beeNodeClient.CashoutChequeForPeerAsync(peerId);

            // Wait for avoid interferences with next tests.
            await Task.Delay(180000);
        }

        [Fact]
        public async Task DepositIntoChequebookAsync()
        {
            // Arrange 
            var originalChequebookBalance = await beeNodeClient.GetChequebookBalanceAsync();
            var amount = 123;


            // Act 
            var result = await beeNodeClient.DepositIntoChequebookAsync(amount);
            await Task.Delay(180000);


            // Assert 
            var actualChequebookBalance = await beeNodeClient.GetChequebookBalanceAsync();
            Assert.Equal(originalChequebookBalance.AvailableBalance + amount, actualChequebookBalance.AvailableBalance);
            Assert.Equal(originalChequebookBalance.TotalBalance + amount, actualChequebookBalance.TotalBalance);
        }

        [Fact]
        public async Task GetAllChequebookChequesAsync()
        {
            // Arrange 
            var allCheques = await beeNodeClient.GetAllChequebookChequesAsync();
            var peerId = allCheques.ToList().First().Peer;


            // Act 
            var allCheque = await beeNodeClient.GetAllChequebookChequesAsync(); //TODO this call return only one peer


            // Assert 
            Assert.Contains(allCheque, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetChequebookAddressAsync()
        {
            // Arrange 


            // Act 
            var cheque = await beeNodeClient.GetChequebookAddressAsync();


            // Assert 
        }

        [Fact]
        public async Task GetChequebookBalanceAsync()
        {
            // Arrange 

            // Act 
            await beeNodeClient.GetChequebookBalanceAsync();
        }

        [Fact]
        public async Task GetChequebookCashoutForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequebookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var chequeBookBalance = await beeNodeClient.GetChequebookCashoutForPeerAsync(peerId);


            // Assert 
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task GetChequebookChequeForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.GetAllChequebookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var chequeBookBalance = await beeNodeClient.GetChequebookChequeForPeerAsync(peerId);


            // Assert
            Assert.True(chequeBookBalance.LastReceived.Payout > 0);
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task WithdrawFromChequebookAsync()
        {
            // Arrange 
            var amount = 123;
            await beeNodeClient.DepositIntoChequebookAsync(amount + 10);
            await Task.Delay(180000);
            var originalChequebookBalance = await beeNodeClient.GetChequebookBalanceAsync();


            // Act 
            var result = await beeNodeClient.WithdrawFromChequebookAsync(amount);
            await Task.Delay(180000);


            // Assert 
            var actualChequebookBalance = await beeNodeClient.GetChequebookBalanceAsync();
            Assert.Equal(originalChequebookBalance.AvailableBalance - amount, actualChequebookBalance.AvailableBalance);
            Assert.Equal(originalChequebookBalance.TotalBalance - amount, actualChequebookBalance.TotalBalance);
        }
    }
}
