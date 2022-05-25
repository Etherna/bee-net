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
            var enumerator = peers.GetEnumerator();
            enumerator.MoveNext();
            var peerId = enumerator.Current;

            // Act 
            var result = await beeNodeClient.DebugClient.CashoutChequeForPeerAsync(peerId);


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
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookCashoutForPeerAsync("51f958d7962f66b1dfe555f41ec00b02610cf9ce1aea01b28eb2115695778d6f");


            // Assert 
        }

        [Fact]
        public async Task GetChequeBookChequeForPeerAsync()
        {
            // Arrange 


            // Act 
            var chequeBookBalance = await beeNodeClient.DebugClient.GetChequeBookChequeForPeerAsync("51f958d7962f66b1dfe555f41ec00b02610cf9ce1aea01b28eb2115695778d6f");


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
