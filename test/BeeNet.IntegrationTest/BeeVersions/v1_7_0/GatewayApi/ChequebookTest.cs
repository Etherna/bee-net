using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.GatewayApi
{
    public class ChequebookTest : BaseTest_Gateway_v3_0_2
    {

        [Fact]
        public async Task CashoutChequeForPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.GatewayClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var result = await beeNodeClient.GatewayClient.CashoutChequeForPeerAsync(peerId); //TODO this call return 500 "message": "cannot cash cheque"
            //TODO when return 500 there are some problems to deserialize the error message to DTO


            // Assert 
            Assert.StartsWith("0x", result);
        }

        [Fact]
        public async Task DepositIntoChequeBookAsync()
        {
            // Arrange 
            var originalChequeBookBalance = await beeNodeClient.GatewayClient.GetChequeBookBalanceAsync();
            var amount = 123;


            // Act 
            var result = await beeNodeClient.GatewayClient.DepositIntoChequeBookAsync(amount);
            await Task.Delay(90000);


            // Assert 
            Assert.StartsWith("0x", result);
            var actualChequeBookBalance = await beeNodeClient.GatewayClient.GetChequeBookBalanceAsync();
            Assert.Equal(originalChequeBookBalance.AvailableBalance + amount, actualChequeBookBalance.AvailableBalance);
            Assert.Equal(originalChequeBookBalance.TotalBalance + amount, actualChequeBookBalance.TotalBalance);
        }

        [Fact]
        public async Task GetAllChequeBookChequesAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var allCheque = await beeNodeClient.GatewayClient.GetAllChequeBookChequesAsync();


            // Assert 
            Assert.Contains(allCheque, i => i.Peer == peerId);
        }

        [Fact]
        public async Task GetChequeBookAddressAsync()
        {
            // Arrange 


            // Act 
            var cheque = await beeNodeClient.GatewayClient.GetChequeBookAddressAsync();


            // Assert 
            Assert.StartsWith("0x", cheque);
        }

        [Fact]
        public async Task GetChequeBookBalanceAsync()
        {
            // Arrange 


            // Act 
            var chequeBookBalance = await beeNodeClient.GatewayClient.GetChequeBookBalanceAsync();


            // Assert 
            Assert.True(chequeBookBalance.TotalBalance > 0);
            Assert.True(chequeBookBalance.AvailableBalance > 0);
        }

        [Fact]
        public async Task GetChequeBookCashoutForPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var chequeBookBalance = await beeNodeClient.GatewayClient.GetChequeBookCashoutForPeerAsync(peerId);


            // Assert 
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task GetChequeBookChequeForPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();
            var peerId = peers.ToList().First();


            // Act 
            var chequeBookBalance = await beeNodeClient.GatewayClient.GetChequeBookChequeForPeerAsync(peerId);


            // Assert
            Assert.True(chequeBookBalance.LastReceived.Payout > 0);
            Assert.Equal(peerId, chequeBookBalance.Peer);
        }

        [Fact]
        public async Task WithdrawFromChequeBookAsync()
        {
            // Arrange 
            var originalChequeBookBalance = await beeNodeClient.GatewayClient.GetChequeBookBalanceAsync();
            var amount = 123;


            // Act 
            var result = await beeNodeClient.GatewayClient.WithdrawFromChequeBookAsync(amount);
            await Task.Delay(90000);


            // Assert 
            Assert.StartsWith("0x", result);
            var actualChequeBookBalance = await beeNodeClient.GatewayClient.GetChequeBookBalanceAsync();
            Assert.Equal(originalChequeBookBalance.AvailableBalance - amount, actualChequeBookBalance.AvailableBalance);
            Assert.Equal(originalChequeBookBalance.TotalBalance - amount, actualChequeBookBalance.TotalBalance);
        }

    }
}
