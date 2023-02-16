using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_12_0.DebugApi
{
    public class ChequebookTest : BaseTest_Debug_v4_0_0
    {

        [Fact]
        public async Task CashoutChequeForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;

            // Act 
            var result = await beeNodeClient.DebugClient.CashoutChequeForPeerAsync(peerId);

            // Assert 
            Assert.StartsWith("0x", result);

            // Wait for avoid interferences with next tests.
            await Task.Delay(180000);
        }

        [Fact]
        public async Task DepositIntoChequeBookAsync()
        {
            // Arrange 
            var originalChequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();
            var amount = 123;


            // Act 
            var result = await beeNodeClient.DebugClient.DepositIntoChequeBookAsync(amount);
            await Task.Delay(180000);


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
            var allCheques = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();
            var peerId = allCheques.ToList().First().Peer;


            // Act 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync(); //TODO this call return only one peer


            // Assert 
            Assert.Contains(allCheque, i => i.Peer == peerId);
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
        }

        [Fact]
        public async Task GetChequeBookCashoutForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookCashoutForPeerAsync(peerId);


            // Assert 
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task GetChequeBookChequeForPeerAsync()
        {
            // Arrange 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();
            var peerId = allCheque.ToList().First().Peer;


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookChequeForPeerAsync(peerId);


            // Assert
            Assert.True(chequeBookBalance.LastReceived.Payout > 0);
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task WithdrawFromChequeBookAsync()
        {
            // Arrange 
            var amount = 123;
            await beeNodeClient.DebugClient.DepositIntoChequeBookAsync(amount + 10);
            await Task.Delay(180000);
            var originalChequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();


            // Act 
            var result = await beeNodeClient.DebugClient.WithdrawFromChequeBookAsync(amount);
            await Task.Delay(180000);


            // Assert 
            Assert.StartsWith("0x", result);
            var actualChequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();
            Assert.Equal(originalChequeBookBalance.AvailableBalance - amount, actualChequeBookBalance.AvailableBalance);
            Assert.Equal(originalChequeBookBalance.TotalBalance - amount, actualChequeBookBalance.TotalBalance);
        }

    }
}
