using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v2_0_0
{
    public class ChequebookTest
    {
        private readonly BeeNodeClient beeNodeClient;
        private string pathTestFileForUpload = "Data\\TestFileForUpload.txt";

        public ChequebookTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

        [Fact]
        public async Task CashoutChequeForPeerAsync()
        {
            // Arrange 
            var peers = await beeNodeClient.DebugClient.GetAllPeerAddressesAsync();

            // Act 
            var cashout = await beeNodeClient.DebugClient.CashoutChequeForPeerAsync("peerId");


            // Assert 
        }

        [Fact]
        public async Task DepositIntoChequeBookAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.DepositIntoChequeBookAsync(123);


            // Assert 
        }

        [Fact]
        public async Task GetAllChequeBookChequesAsync()
        {
            // Arrange 


            // Act 
            var allCheque = await beeNodeClient.DebugClient.GetAllChequeBookChequesAsync();


            // Assert 
        }

        [Fact]
        public async Task GetChequeBookAddressAsync()
        {
            // Arrange 


            // Act 
            var cheque = await beeNodeClient.DebugClient.GetChequeBookAddressAsync();


            // Assert 
        }

        [Fact]
        public async Task GetChequeBookBalanceAsync()
        {
            // Arrange 


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookBalanceAsync();


            // Assert 
        }

        [Fact]
        public async Task GetChequeBookCashoutForPeerAsync()
        {
            // Arrange 


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookCashoutForPeerAsync("peerId");


            // Assert 
        }

        [Fact]
        public async Task GetChequeBookChequeForPeerAsync()
        {
            // Arrange 


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookChequeForPeerAsync("peerId");


            // Assert 
        }

        [Fact]
        public async Task WithdrawFromChequeBookAsync()
        {
            // Arrange 


            // Act 
            var result = await beeNodeClient.DebugClient.WithdrawFromChequeBookAsync(123);


            // Assert 
        }
    }
}
